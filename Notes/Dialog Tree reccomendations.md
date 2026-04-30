# Dialog Tree System: Best Practices and Gotchas

Dialog trees seem simple until you've shipped one. Here's what tends to bite people, organized by the order in which problems usually emerge.

## Architecture Foundations

**Separate data from logic.** Dialog content should live in data files (JSON, YAML, custom DSL, or a tool like Yarn/Ink) — not hardcoded. This lets writers iterate without touching code, enables localization, and makes diffs reviewable. The runtime is just an interpreter.

**Make nodes addressable by stable IDs, not by position.** If you reference nodes by array index or line number, every insertion breaks save games and cross-references. Use string IDs that survive reordering.

**Decide early: tree, graph, or state machine?** You'll almost certainly want a directed graph (nodes can be revisited, multiple paths can converge). Systems benefit from a state machine layer on top for "conversation modes." Pretending you have a tree when you have a graph leads to duplicate content and broken back-references.

## The Conditions and Effects Layer

This is where most systems get messy. Each dialog choice typically has:
- **Conditions** for whether it appears or is selectable
- **Effects** that fire when chosen (set flags, give items, trigger events)

Best practices:
- Use a small expression language rather than hardcoded condition types. `has_item("key") && quest("rescue").stage >= 2` scales better than enum-based conditions.
- Distinguish "hidden" from "shown but disabled" — both are useful and players notice the difference.
- Make effects idempotent where possible, or track whether they've fired. Players will reload, retry, and exploit anything that gives rewards repeatedly.
- Log every condition evaluation in debug builds. You'll need it.

## Commonly Missed Gotchas

**Variable interpolation timing.** When does `"Hello, {player_name}!"` get resolved — at authoring, at node entry, or at display? If the player renames themselves mid-conversation, what happens? Pick one and document it.

**Choice ordering and stability.** If conditions hide some choices, do the visible ones renumber? Players using "always pick option 2" muscle memory will hate you. Consider stable slots vs. compacted lists as a design decision, not an accident.

**Re-entry state.** What happens if the player walks away mid-conversation and comes back? Does the NPC resume, restart, or pick a contextual greeting? Saving mid-dialog is especially nasty — you need to serialize the current node, any pending effects, and the conversation's local variables.

**Localization length variance.** German is ~30% longer than English; Japanese can be much shorter but needs different line-break rules. UI that fits English will overflow. Also: pluralization rules differ wildly (Russian has multiple plural forms based on number), and gendered grammar means `{player_name} is ready` may need entirely different sentence structures per language. Plan for ICU MessageFormat or equivalent from day one.

**Voice acting constraints.** Once lines are recorded, changing them is expensive. This means:
  - Lock text before VO, or budget for pickups
  - Variable interpolation in VO lines is mostly impossible (you can't record `{player_name}`)
  - Line IDs must be stable forever — never reuse them
  - You need a pipeline that flags when text changes after recording

**Skip and auto-advance.** Players will mash through dialog. Make sure:
  - Skipping doesn't skip *effects* (granting items, advancing quests)
  - Skipping a choice prompt either auto-selects a default or pauses — don't let it advance silently
  - Auto-advance timing accounts for text length, not a fixed delay

**Interruptions.** Combat starts, a cutscene triggers, the player closes the menu. Dialog systems that assume they own the screen until completion break in surprising ways. Build in a clean "abort with state cleanup" path.

**Cyclic content and "I've heard this."** When the player revisits an NPC, you almost always want different greetings on the second, third, and Nth visit. Build a "times visited" or "times said" counter into the system rather than retrofitting it. Related: "barks" (short contextual one-liners) often want to share infrastructure with full dialogs but have different rules — design that boundary deliberately.

## Special Cases Worth Anticipating

**Branching that needs to converge.** Three choices, three different responses, then everyone ends up at the same next node. Your data format needs to express this without forcing you to duplicate the convergence subtree. Goto-style references work but make graphs hard to visualize.

**Conditional text within a single line.** "I haven't seen you in [days/weeks/months]." This is finer-grained than node-level branching and usually needs an inline templating syntax. Decide whether to support it — adding it later means rewriting content.

**Multi-party conversations.** Two NPCs talking to each other while the player watches, or a three-way conversation where the player picks responses. The "current speaker" becomes a variable and your UI needs to handle portrait swaps, off-screen speakers, and possibly simultaneous speech.

**Dialog during gameplay.** Walking-and-talking, combat banter, dialog in vehicles. These usually can't use the full menu UI and need a stripped-down "subtitle mode." Sharing data between systems is good; sharing UI assumptions is bad.

**Player-initiated topic selection.** "Ask about: the war / the king / the missing ring." This is a different UI pattern than reactive choices and often wants its own node type. Topics may unlock, lock, or change based on world state, and players expect to revisit them.

**Time-pressured choices.** A choice with a countdown that auto-selects on timeout. The timeout itself is a "choice" for save/load purposes and needs to serialize cleanly.

**Choices that affect *future* dialog availability, not just immediate response.** This is the entire point of branching narrative, but it means your flag/variable system needs to be queryable from anywhere in the dialog tree, not just locally scoped.

**Debug and authoring tools.** You will need: a way to jump to any node, a way to set arbitrary flags, a visualizer that shows the graph (Twine-style), and a linter that catches unreachable nodes, broken references, and conditions that can never be true. Building these early pays for itself many times over.

## A Pragmatic Suggestion

Before building your own, look hard at **Yarn Spinner** and **Ink** — both are mature, well-documented, and free. Even if you end up rolling your own, studying their data formats, command syntax, and runtime APIs will surface design decisions you didn't know you needed to make. The "obvious" approach to dialog trees has been refined considerably by people who've shipped games on top of these systems.

Want me to go deeper on any specific area — the expression language design, save/load handling, the localization pipeline, or the authoring tools?
