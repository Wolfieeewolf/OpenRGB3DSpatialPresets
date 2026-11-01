# Patterns

1-D strip / kernel patterns. Each `.kernel` file defines a short GLSL fragment that sets `k` in `[-1, 1]`. Available globals: `s01`, `r`, `ph`, `u_phase`, `tsec`, `TWO_PI`, `hash11()`, `smstep()`, `fractf()`.

Index numbers must be unique across all kernels (used for cross-referencing in presets and shell-pattern).
