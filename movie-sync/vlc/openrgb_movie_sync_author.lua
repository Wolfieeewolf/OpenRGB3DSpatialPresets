-- OpenRGB 3D Spatial - Movie Sync cue author (VLC Lua EXTENSION)
-- Install: copy to %APPDATA%/vlc/lua/extensions/openrgb_movie_sync_author.lua
-- View → Add Interface is NOT required; enable under View → OpenRGB Movie Sync Author
-- (or Extensions menu, depending on VLC build).
--
-- Requires OpenRGB with Movie Sync running and Script API on (default port 27180).
-- Marks start/end while the video plays, picks a catalog look, writes
-- <video>.movie_sync.json beside the media (creates the file if missing).

local DEFAULT_PORT = 27180

local state = {
  port = DEFAULT_PORT,
  effects = {}, -- { id=, name=, category=, label= }
  mark_start = nil,
  mark_end = nil,
  dlg = nil,
  dd_effect = nil,
  lbl_time = nil,
  lbl_marks = nil,
  lbl_status = nil,
  txt_port = nil,
}

local function msg_info(s)
  vlc.msg.info("[OpenRGB Author] " .. s)
end

local function msg_warn(s)
  vlc.msg.warn("[OpenRGB Author] " .. s)
end

local function set_status(s)
  if state.lbl_status then
    state.lbl_status:set_text(s)
  end
  msg_info(s)
end

local function http_exchange(port, request)
  local body = nil
  local ok, err = pcall(function()
    local sock = vlc.net.connect_tcp("127.0.0.1", port)
    if not sock then
      error("connect failed")
    end
    vlc.net.send(sock, request)
    local chunks = {}
    -- VLC Lua: recv returns nil/empty when the peer closes.
    for _ = 1, 64 do
      local chunk = vlc.net.recv(sock)
      if not chunk or chunk == "" then
        break
      end
      table.insert(chunks, chunk)
      if #chunk < 1024 and table.concat(chunks):find("\r\n\r\n") then
        -- likely complete for small JSON responses; keep reading a bit more
      end
    end
    vlc.net.close(sock)
    local data = table.concat(chunks)
    body = data:match("\r\n\r\n(.*)$") or ""
  end)
  if not ok then
    return nil, tostring(err)
  end
  return body, nil
end

local function http_get(port, path)
  local req = string.format(
    "GET %s HTTP/1.1\r\nHost: 127.0.0.1\r\nConnection: close\r\n\r\n",
    path)
  return http_exchange(port, req)
end

local function http_post(port, path, json_body)
  local req = string.format(
    "POST %s HTTP/1.1\r\nHost: 127.0.0.1\r\nContent-Type: application/json\r\nContent-Length: %d\r\nConnection: close\r\n\r\n%s",
    path, #json_body, json_body)
  return http_exchange(port, req)
end

local function json_escape(s)
  s = tostring(s or "")
  s = s:gsub("\\", "\\\\")
  s = s:gsub('"', '\\"')
  s = s:gsub("\n", "\\n")
  s = s:gsub("\r", "\\r")
  return s
end

local function parse_catalog(body)
  local list = {}
  if not body or body == "" then
    return list
  end
  -- Prefer the effects array so the outer {"effects":[...]} object is not treated as a look.
  local arr = body:match('"effects"%s*:%s*(%b[])') or body
  for block in arr:gmatch("%b{}") do
    local id = block:match('"id"%s*:%s*"(.-)"')
    if id and id ~= "" then
      local name = block:match('"name"%s*:%s*"(.-)"') or id
      local category = block:match('"category"%s*:%s*"(.-)"') or ""
      local label = name
      if category ~= "" then
        label = string.format("[%s] %s", category, name)
      end
      table.insert(list, {
        id = id,
        name = name,
        category = category,
        label = label,
      })
    end
  end
  table.sort(list, function(a, b)
    if a.category == b.category then
      return a.name:lower() < b.name:lower()
    end
    return a.category:lower() < b.category:lower()
  end)
  return list
end

local function current_media_path()
  local item = vlc.input.item()
  if not item then
    return nil
  end
  local uri = item:uri()
  if not uri then
    return nil
  end
  local path = vlc.strings.decode_uri(uri)
  path = path:gsub("^file://localhost/", "")
  path = path:gsub("^file:///", "")
  path = path:gsub("^file://", "")
  if path:match("^/%a:/") then
    path = path:sub(2)
  end
  return path
end

local function sidecar_path(media)
  if not media or media == "" then
    return nil
  end
  local norm = media:gsub("\\", "/")
  local base = norm:gsub("%.[^%./]+$", "")
  local path = base .. ".movie_sync.json"
  if media:find("\\") then
    path = path:gsub("/", "\\")
  end
  return path
end

local function input_ms()
  local input = vlc.object.input()
  if not input then
    return nil
  end
  local t = vlc.var.get(input, "time")
  if t == nil then
    return nil
  end
  return math.floor(t / 1000)
end

local function media_duration_ms()
  local input = vlc.object.input()
  if not input then
    return nil
  end
  local len = vlc.var.get(input, "length")
  if len == nil then
    return nil
  end
  return math.floor(len / 1000)
end

local function read_file(path)
  local f = io.open(path, "rb")
  if not f then
    return nil
  end
  local data = f:read("*a")
  f:close()
  return data
end

local function write_file(path, data)
  local f = io.open(path, "wb")
  if not f then
    return false
  end
  f:write(data)
  f:close()
  return true
end

local function parse_cues(data)
  local list = {}
  if not data then
    return list
  end
  for t, tend, id, edge in data:gmatch(
      '"t_ms"%s*:%s*(%d+).-"t_end_ms"%s*:%s*(%d+).-"effect_id"%s*:%s*"(.-)".-"edge"%s*:%s*"(.-)"') do
    table.insert(list, {
      t_ms = tonumber(t),
      t_end_ms = tonumber(tend),
      effect_id = id,
      edge = edge,
    })
  end
  for t, id, edge in data:gmatch(
      '"t_ms"%s*:%s*(%d+).-"effect_id"%s*:%s*"(.-)".-"edge"%s*:%s*"(.-)"') do
    local exists = false
    for _, c in ipairs(list) do
      if c.effect_id == id and c.t_ms == tonumber(t) then
        exists = true
        break
      end
    end
    if not exists then
      table.insert(list, {
        t_ms = tonumber(t),
        effect_id = id,
        edge = edge,
      })
    end
  end
  table.sort(list, function(a, b)
    return a.t_ms < b.t_ms
  end)
  return list
end

local function title_from_media(media)
  if not media then
    return "Untitled"
  end
  local name = media:gsub("\\", "/"):match("([^/]+)$") or "Untitled"
  return name:gsub("%.[^%.]+$", "")
end

local function encode_sidecar(title, port, duration_ms, cues)
  local lines = {}
  table.insert(lines, "{")
  table.insert(lines, '  "format": "openrgb3d.movie_sync",')
  table.insert(lines, '  "version": 1,')
  table.insert(lines, string.format('  "title": "%s",', json_escape(title)))
  table.insert(lines, string.format('  "port": %d,', port))
  if duration_ms and duration_ms > 0 then
    table.insert(lines, string.format('  "duration_ms": %d,', duration_ms))
    table.insert(lines, '  "duration_tolerance_ms": 5000,')
  end
  table.insert(lines, '  "notes": "Authored with OpenRGB Movie Sync Author (VLC)",')
  table.insert(lines, '  "cues": [')
  for i, c in ipairs(cues) do
    local comma = (i < #cues) and "," or ""
    if c.t_end_ms and c.edge == "level" then
      table.insert(lines, string.format(
        '    { "t_ms": %d, "t_end_ms": %d, "effect_id": "%s", "edge": "level" }%s',
        c.t_ms, c.t_end_ms, json_escape(c.effect_id), comma))
    else
      table.insert(lines, string.format(
        '    { "t_ms": %d, "effect_id": "%s", "edge": "%s" }%s',
        c.t_ms, json_escape(c.effect_id), json_escape(c.edge or "pulse"), comma))
    end
  end
  table.insert(lines, "  ]")
  table.insert(lines, "}")
  table.insert(lines, "")
  return table.concat(lines, "\n")
end

local function selected_effect_id()
  if not state.dd_effect or #state.effects == 0 then
    return nil
  end
  local idx = state.dd_effect:get_value()
  if type(idx) ~= "number" or idx < 1 or idx > #state.effects then
    -- Some VLC builds return the string label.
    local label = tostring(idx)
    for _, e in ipairs(state.effects) do
      if e.label == label or e.id == label or e.name == label then
        return e.id
      end
    end
    return state.effects[1] and state.effects[1].id or nil
  end
  return state.effects[idx].id
end

local function refresh_marks_label()
  if not state.lbl_marks then
    return
  end
  local s = state.mark_start and (tostring(state.mark_start) .. " ms") or " - "
  local e = state.mark_end and (tostring(state.mark_end) .. " ms") or " -  (pulse)"
  state.lbl_marks:set_text("Start: " .. s .. "   End: " .. e)
end

local function refresh_time_label()
  if not state.lbl_time then
    return
  end
  local now = input_ms()
  if now then
    state.lbl_time:set_text("Playhead: " .. tostring(now) .. " ms")
  else
    state.lbl_time:set_text("Playhead: (no media)")
  end
end

local function fill_effect_dropdown()
  if not state.dd_effect then
    return
  end
  -- Clear + refill is awkward across VLC versions; recreate labels list.
  local labels = {}
  for _, e in ipairs(state.effects) do
    table.insert(labels, e.label)
  end
  if #labels == 0 then
    table.insert(labels, "(no looks - start Movie Sync Script API)")
  end
  pcall(function()
    state.dd_effect:clear()
  end)
  for i, label in ipairs(labels) do
    state.dd_effect:add_value(label, i)
  end
end

local function reload_catalog()
  local port = tonumber(state.txt_port and state.txt_port:get_text()) or state.port or DEFAULT_PORT
  state.port = port
  local health = http_get(port, "/v1/health")
  if not health or not health:find('"ok"%s*:%s*true') then
    state.effects = {}
    fill_effect_dropdown()
    set_status("OpenRGB Movie Sync Script API not reachable on port " .. tostring(port))
    return
  end
  local body, err = http_get(port, "/v1/catalog")
  if not body then
    state.effects = {}
    fill_effect_dropdown()
    set_status("Catalog failed - " .. tostring(err))
    return
  end
  state.effects = parse_catalog(body)
  fill_effect_dropdown()
  if #state.effects == 0 then
    set_status("Catalog empty - add Movie Sync looks, then Reload catalog")
  else
    set_status(string.format("Loaded %d look(s) from OpenRGB", #state.effects))
  end
end

local function mark_start()
  local now = input_ms()
  if not now then
    set_status("No media playhead - open a video first")
    return
  end
  state.mark_start = now
  if state.mark_end and state.mark_end <= state.mark_start then
    state.mark_end = nil
  end
  refresh_marks_label()
  set_status("Marked start at " .. tostring(now) .. " ms")
end

local function mark_end()
  local now = input_ms()
  if not now then
    set_status("No media playhead - open a video first")
    return
  end
  if not state.mark_start then
    set_status("Mark start first")
    return
  end
  if now <= state.mark_start then
    set_status("End must be after start")
    return
  end
  state.mark_end = now
  refresh_marks_label()
  set_status("Marked end at " .. tostring(now) .. " ms (level hold)")
end

local function clear_end()
  state.mark_end = nil
  refresh_marks_label()
  set_status("Cleared end - next save will be a pulse")
end

local function preview_selected()
  local id = selected_effect_id()
  if not id or id:find("^%(") then
    set_status("Pick a look from the list")
    return
  end
  local port = tonumber(state.txt_port and state.txt_port:get_text()) or state.port or DEFAULT_PORT
  local body = string.format(
    '{"effect_id":"%s","edge":"pulse","active":true}',
    json_escape(id))
  local resp, err = http_post(port, "/v1/effect", body)
  if not resp then
    set_status("Preview failed - " .. tostring(err))
    return
  end
  set_status("Previewed "" .. id .. """)
end

local function save_cue()
  local media = current_media_path()
  if not media then
    set_status("No media path - open a local video file")
    return
  end
  if not state.mark_start then
    set_status("Mark start first")
    return
  end
  local id = selected_effect_id()
  if not id or id:find("^%(") then
    set_status("Pick a look from the catalog list")
    return
  end
  local known = false
  for _, e in ipairs(state.effects) do
    if e.id == id then
      known = true
      break
    end
  end
  if not known then
    set_status("Unknown look id - Reload catalog")
    return
  end

  local edge = "pulse"
  local tend = nil
  if state.mark_end and state.mark_end > state.mark_start then
    edge = "level"
    tend = state.mark_end
  end

  local path = sidecar_path(media)
  local existing = read_file(path)
  local cues = parse_cues(existing)
  local port = tonumber(state.txt_port and state.txt_port:get_text()) or state.port or DEFAULT_PORT
  if existing then
    port = tonumber(existing:match('"port"%s*:%s*(%d+)')) or port
  end

  table.insert(cues, {
    t_ms = state.mark_start,
    t_end_ms = tend,
    effect_id = id,
    edge = edge,
  })
  table.sort(cues, function(a, b)
    return a.t_ms < b.t_ms
  end)

  local title = title_from_media(media)
  if existing then
    local t = existing:match('"title"%s*:%s*"(.-)"')
    if t and t ~= "" then
      title = t
    end
  end
  local duration = media_duration_ms() or 0
  if existing then
    duration = tonumber(existing:match('"duration_ms"%s*:%s*(%d+)')) or duration
  end

  local json = encode_sidecar(title, port, duration, cues)
  if not write_file(path, json) then
    set_status("Could not write " .. tostring(path))
    return
  end

  state.mark_start = nil
  state.mark_end = nil
  refresh_marks_label()
  set_status(string.format("Saved %s cue "%s" - %d cue(s) in sidecar", edge, id, #cues))
end

local function close_dialog()
  if state.dlg then
    state.dlg:delete()
  end
  state.dlg = nil
  state.dd_effect = nil
  state.lbl_time = nil
  state.lbl_marks = nil
  state.lbl_status = nil
  state.txt_port = nil
end

local function open_dialog()
  if state.dlg then
    refresh_time_label()
    return
  end
  local dlg = vlc.dialog("OpenRGB Movie Sync Author")
  state.dlg = dlg

  dlg:add_label("Port (OpenRGB Script API)", 1, 1, 1, 1)
  state.txt_port = dlg:add_text_input(tostring(state.port or DEFAULT_PORT), 2, 1, 1, 1)

  dlg:add_button("Reload catalog", reload_catalog, 1, 2, 1, 1)
  dlg:add_button("Refresh time", refresh_time_label, 2, 2, 1, 1)

  state.lbl_time = dlg:add_label("Playhead:  - ", 1, 3, 2, 1)
  state.lbl_marks = dlg:add_label("Start: -   End: - (pulse)", 1, 4, 2, 1)

  dlg:add_label("Look", 1, 5, 1, 1)
  state.dd_effect = dlg:add_dropdown(2, 5, 1, 1)

  dlg:add_button("Mark start", mark_start, 1, 6, 1, 1)
  dlg:add_button("Mark end", mark_end, 2, 6, 1, 1)
  dlg:add_button("Clear end (pulse)", clear_end, 1, 7, 1, 1)
  dlg:add_button("Preview look", preview_selected, 2, 7, 1, 1)
  dlg:add_button("Save cue", save_cue, 1, 8, 2, 1)

  state.lbl_status = dlg:add_label("Load catalog, mark times, save beside the video.", 1, 9, 2, 1)

  dlg:add_button("Close", close_dialog, 1, 10, 2, 1)

  refresh_time_label()
  refresh_marks_label()
  reload_catalog()
  dlg:show()
end

-- VLC extension API ---------------------------------------------------------

function descriptor()
  return {
    title = "OpenRGB Movie Sync Author",
    version = "0.1",
    author = "OpenRGB 3D Spatial",
    shortdesc = "Mark Movie Sync cues while watching",
    description = "Mark start/end on the VLC playhead, pick a look from the OpenRGB Movie Sync catalog, and write <video>.movie_sync.json beside the file.",
    capabilities = { "menu" },
  }
end

function activate()
  open_dialog()
end

function deactivate()
  close_dialog()
end

function close()
  close_dialog()
end

function menu()
  return {
    "Open author panel",
    "Mark start",
    "Mark end",
    "Clear end (pulse)",
    "Save cue",
    "Reload catalog",
  }
end

function trigger_menu(id)
  if id == 1 then
    open_dialog()
  elseif id == 2 then
    if not state.dlg then
      open_dialog()
    end
    mark_start()
  elseif id == 3 then
    if not state.dlg then
      open_dialog()
    end
    mark_end()
  elseif id == 4 then
    clear_end()
  elseif id == 5 then
    if not state.dlg then
      open_dialog()
    end
    save_cue()
  elseif id == 6 then
    if not state.dlg then
      open_dialog()
    end
    reload_catalog()
  end
end
