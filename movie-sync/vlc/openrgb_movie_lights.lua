-- OpenRGB 3D Spatial — Movie Lights (VLC Lua extension)
-- View → OpenRGB Movie Lights after install. Bring your own media.

function descriptor()
  return {
    title = "OpenRGB Movie Lights",
    version = "0.1",
    author = "OpenRGB3DSpatial",
    url = "https://github.com/Wolfieeewolf/OpenRGB3DSpatialPlugin",
    shortdesc = "Fire shared movie-effect catalog cues into OpenRGB",
    description = "Loads a .movie_cues.json script and POSTs effect_id cues to the OpenRGB Movie Scripts API.",
    capabilities = {"menu", "input-listener"}
  }
end

local port = 27180
local script_path = nil
local cues = {}
local edition = {}
local file_expect = {}
local dvd = {}
local duration_ms = nil
local duration_tol = 5000
local armed = false
local next_idx = 1
local offset_ms = 0
local last_title = -1

local function http_post(body)
  local ok, err = pcall(function()
    local sock = vlc.net.connect_tcp("127.0.0.1", port)
    if not sock then return end
    local req = "POST /v1/effect HTTP/1.1\r\nHost: 127.0.0.1\r\nContent-Type: application/json\r\nContent-Length: " .. #body .. "\r\nConnection: close\r\n\r\n" .. body
    vlc.net.send(sock, req)
    vlc.net.close(sock)
  end)
  return ok
end

local function fire(effect_id, edge, active)
  local body = string.format('{"effect_id":"%s","edge":"%s","active":%s}', effect_id, edge or "pulse", active and "true" or "false")
  http_post(body)
end

local function parse_json_file(path)
  local f = io.open(path, "rb")
  if not f then return nil end
  local data = f:read("*a")
  f:close()
  -- Minimal cue extractor (expects pretty or compact JSON from our schema).
  local list = {}
  for t, id, edge in data:gmatch('"t_ms"%s*:%s*(%d+).- "effect_id"%s*:%s*"(.-)".-"edge"%s*:%s*"(.-)"') do
    table.insert(list, { t_ms = tonumber(t), effect_id = id, edge = edge, active = true })
  end
  if #list == 0 then
    for t, id in data:gmatch('"t_ms"%s*:%s*(%d+).- "effect_id"%s*:%s*"(.-)"') do
      table.insert(list, { t_ms = tonumber(t), effect_id = id, edge = "pulse", active = true })
    end
  end
  table.sort(list, function(a, b) return a.t_ms < b.t_ms end)
  local dur = data:match('"duration_ms"%s*:%s*(%d+)')
  local tol = data:match('"duration_tolerance_ms"%s*:%s*(%d+)')
  local feat = data:match('"feature_title"%s*:%s*(%d+)')
  local sch = data:match('"start_chapter"%s*:%s*(%d+)')
  local std = data:match('"video_standard"%s*:%s*"(.-)"')
  local region = data:match('"region"%s*:%s*"(.-)"')
  return list, tonumber(dur), tonumber(tol) or 5000, tonumber(feat), tonumber(sch) or 1, std, region
end

local function input_ms()
  local input = vlc.object.input()
  if not input then return nil end
  local t = vlc.var.get(input, "time")
  if not t then return nil end
  return math.floor(t / 1000)
end

local function input_length_ms()
  local input = vlc.object.input()
  if not input then return nil end
  local l = vlc.var.get(input, "length")
  if not l then return nil end
  return math.floor(l / 1000)
end

local function current_title_chapter()
  local input = vlc.object.input()
  if not input then return nil, nil end
  local title = vlc.var.get(input, "title")
  local chapter = vlc.var.get(input, "chapter")
  return title, chapter
end

function activate()
  vlc.msg.info("[OpenRGB Movie Lights] active")
end

function deactivate()
  armed = false
end

function meta_changed()
end

local function load_script_dialog()
  local path = vlc.dialog().file_picker(false, "Movie cue script", "JSON files (*.movie_cues.json;*.json)|*.movie_cues.json;*.json|All (*.*)|*.*")
  if not path or path == "" then return end
  script_path = path
  local list, dur, tol, feat, sch, std, region = parse_json_file(path)
  cues = list or {}
  duration_ms = dur
  duration_tol = tol
  dvd.feature_title = feat
  dvd.start_chapter = sch
  edition.video_standard = std
  edition.region = region
  next_idx = 1
  armed = true
  local len = input_length_ms()
  if duration_ms and len then
    local delta = math.abs(len - duration_ms)
    if delta > duration_tol then
      vlc.msg.warn(string.format("[OpenRGB Movie Lights] Duration mismatch: file ~%dms, script expects %dms (±%d). Sync may be wrong.", len, duration_ms, duration_tol))
    end
  end
  vlc.msg.info(string.format("[OpenRGB Movie Lights] Loaded %d cues from %s", #cues, path))
end

function menu()
  return { "Load script…", "Arm / resume cues", "Disarm", "Fire test explosion" }
end

function trigger_menu(id)
  if id == 1 then
    load_script_dialog()
  elseif id == 2 then
    armed = true
  elseif id == 3 then
    armed = false
  elseif id == 4 then
    fire("explosion", "pulse", true)
  end
end

function input_changed()
  next_idx = 1
  last_title = -1
end

function playing_changed(status)
  -- 2 = paused in some VLC versions; keep simple
end

function tick()
  if not armed or #cues == 0 then return end

  if dvd.feature_title then
    local title, chapter = current_title_chapter()
    if title ~= nil then
      if title ~= dvd.feature_title then
        return
      end
      if chapter ~= nil and chapter + 1 < (dvd.start_chapter or 1) then
        return
      end
      if title ~= last_title then
        next_idx = 1
        last_title = title
      end
    end
  end

  local now = input_ms()
  if not now then return end
  now = now + offset_ms
  while next_idx <= #cues and cues[next_idx].t_ms <= now do
    local c = cues[next_idx]
    fire(c.effect_id, c.edge or "pulse", true)
    next_idx = next_idx + 1
  end
end
