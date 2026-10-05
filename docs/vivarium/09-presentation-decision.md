# Presentation shell decision

For the Slice 10 proof-of-concept, LivingSim-Eco retains a deliberately dependency-free terminal renderer rather than prematurely committing to a desktop game framework. This is a product decision for the foundation milestone, not a claim that terminal rendering is the eventual art direction.

The proven observer requirements are now met by the shell: four zoom levels, eased camera and animal presentation, seasonal/climate/resource overlays, distinct species glyphs, population and inspection panels, history focus, pause, and fast-forward up to 10,000 simulation ticks per frame. The simulation remains headless and contains no presentation dependency.

The next product roadmap should prototype desktop and graphical renderers against these requirements before selecting a permanent rendering stack.
