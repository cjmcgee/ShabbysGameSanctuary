# Tile Types Plan

Inventory of tile types likely needed to render every area described in [MAP.md](MAP.md). Tiles are grouped by category; each entry notes the area(s) that drive the requirement so you can prioritize. Existing entries from [RetroSystems/TileType.cs](RetroSystems/TileType.cs) are marked **(existing)** — everything else is a proposal.

Naming follows the current semantic-enum convention (one logical concept; per-system art renders it). Where a single concept has meaningful variants the kids would notice (e.g. Victorian vs. ranch home), I list them as separate entries rather than relying on the per-house `Accent` tint, since the silhouette/roofline is what reads at low resolution.

## 1. Ground & terrain (outdoor base layer)

| Tile | Notes / where used |
|------|---------------------|
| `Grass` **(existing)** | Default lawn / yard fill |
| `GrassAlt` **(existing)** | Variant for break-up / wild patches |
| `TallGrass` | River bank wild rice, fallow farm edges |
| `Dirt` | Woods trails, dirt bike track, primary school yard |
| `Gravel` | School yards ("dust and gravel base"), dirt track |
| `Sand` | Creek bed, sandbox at primary school |
| `Mud` | Creek edge, rainy spots |
| `Rock` | Rocky butte surface, creek stones |
| `Boulder` | Butte outcrops, scattered woods |
| `CliffFace` | Side of butte, meditation center backdrop |
| `Cobblestone` | Old downtown streets dating back to 1800s |
| `CobblePatched` | Cobblestone with blacktop patches (downtown reality) |
| `Snow` | Winter variant (river freezes, harsh weather mentioned) |
| `Ice` | Frozen-over river in winter |
| `PlowedField` | Farm land active rows |
| `FallowField` | Farm land bare/fallow seasons |
| `CornCrop` | Tall summer corn rows |
| `WheatCrop` | Golden field rows |

## 2. Roads, paths, parking

| Tile | Notes |
|------|-------|
| `Road` **(existing)** | Main asphalt road through town |
| `Sidewalk` **(existing)** | Suburban / downtown walks |
| `RoadStripe` | Lane / center-line variant |
| `Crosswalk` | Major-road intersections |
| `ParkingStripe` | Strip mall, arcade, fairgrounds lots |
| `ParkingDecayed` | Arcade's "decaying parking lot", shabby bar |
| `DirtPath` | Convent grounds path, woods trails |
| `BridgeWood` | Cute little bridges over the convent pond |
| `BridgeRoad` | Major-road bridge over the river |
| `RailroadTrack` | Rail line past fairgrounds + middle/high school |
| `RailroadTie` | Cross-tie variant for siding/yard art |
| `TrainPlatform` | Station platforms |

## 3. Water

| Tile | Notes |
|------|-------|
| `WaterRiver` | Wide, peaceful, north–south river |
| `WaterLake` | Murky vast lake, eastern border |
| `WaterCreek` | Babbling shallow creek in the woods |
| `WaterPond` | Convent pond with shrine island |
| `WaterPool` | Chlorinated neighborhood pool |
| `WaterEdge` | Shoreline / waterline transition (works for any of the above) |
| `RetainingWall` | Concrete walls penning in the river bank |
| `Dock` | Boat rental / lake clubhouse |
| `LilyPad` | Pond decoration |
| `Cattails` | Pond / river edge greenery |

## 4. Vegetation & natural decoration

| Tile | Notes |
|------|-------|
| `Bush` **(existing)** | Generic shrub |
| `TreeDeciduous` | Woods canopy ("large deciduous trees") |
| `TreeEvergreen` | Mixed-in conifers, butte slopes |
| `TreeStump` | Cleared underbrush in campground |
| `FallenLog` | Woods scenery |
| `Hedge` | Private school landscaping |
| `Flowers` | Convent grounds, well-kept lawns |
| `WildVines` | Wild grapes on the river bank |
| `Roots` | Exposed roots along creek bank (called out specifically) |
| `Mushroom` | Woods detail |

## 5. Building exteriors — walls

The town has at least three distinct architectural feels: Victorian downtown, mixed suburban (Victorian/Tudor/Federal/Craftsman/Gothic revival/cottage), and modern big-box. Use these as silhouette-distinct tiles; let `Accent` keep handling per-building tint.

| Tile | Notes |
|------|-------|
| `HouseExterior` **(existing)** | Generic suburban siding |
| `HouseVictorian` | Downtown row homes, suburban Victorians |
| `HouseTudor` | Tudor-style suburban home |
| `HouseCraftsman` | Craftsman bungalow |
| `HouseCottage` | Small cottage |
| `HouseFederal` | Symmetric brick/painted federal |
| `HouseGothicRevival` | Pointed-arch suburban accent |
| `BrickWall` | Downtown shops, town hall, library |
| `StoneWall` | Private school perimeter, meditation center |
| `WoodPlankWall` | Pub, bait shop, shabby bar |
| `StuccoWall` | Strip mall, fast food, gas station |
| `BigBoxWall` | Department store, fancy gym |
| `IndustrialWall` | Maintenance shed, auto garage |
| `BlackoutWall` | Arcade exterior (black with neon accents — special art) |
| `CamoWall` | Bait shop's namesake camouflage paint |
| `GraffitiWall` | Variant for arcade dumpster area |

## 6. Roofs & rooflines

| Tile | Notes |
|------|-------|
| `HouseRoof` **(existing)** | Default shingle |
| `RoofSlate` | Church, fancier downtown buildings |
| `RoofFlat` | Strip mall, big box, gas station |
| `RoofMetal` | Sheds, garage, train station |
| `Steeple` | Church + private school chapel |
| `Spire` | Church flanks |
| `GreenhouseRoof` | Glass roof tile (school greenhouse) |
| `PoleShelter` | Campground picnic shelter |

## 7. Doors, windows, storefronts

| Tile | Notes |
|------|-------|
| `Door` **(existing)** | Generic door |
| `DoorOrnate` | Church grand doorway |
| `DoorDouble` | Town hall, schools, museum |
| `DoorGarage` | Auto repair bay |
| `DoorStorefront` | Glass shop entrance |
| `Window` **(existing)** | Generic |
| `WindowStorefront` | Big shop windows full of posters (bait shop, donut, electronics) |
| `WindowBlackedOut` | Arcade, pub, shabby bar |
| `WindowStainedGlass` | Church, chapel back wall |
| `Awning` | Diner, downtown shops |
| `Marquee` | Movie theater |

## 8. Fences, walls, boundaries

| Tile | Notes |
|------|-------|
| `FencePicket` | Suburban yards |
| `FenceChainlink` | School yards, dirt track perimeter |
| `FenceWroughtIron` | Cemetery (with dangerous finials) |
| `FenceSplitRail` | Farm land |
| `FenceWire` | Farm land alt |
| `WallStoneTall` | Private school's high stone perimeter |
| `Gate` | Cemetery, school, farm |
| `Bollard` | Park / lot perimeter |

## 9. Interior floors

| Tile | Notes |
|------|-------|
| `WoodFloor` **(existing)** | Home, church |
| `Carpet` **(existing)** | Home, doctor offices |
| `KitchenTile` **(existing)** | Home kitchens |
| `Linoleum` | Diner, school, doctor |
| `CheckerTile` | Diner 50s aesthetic |
| `ConcreteFloor` | Garage, maintenance shed, primary school basement |
| `BlackSteelFloor` | Primary school's coal-bunker tornado shelter (called out specifically) |
| `LockerRoomTile` | Gyms |
| `BowlingLane` | Bowling alley |
| `StagePolish` | Theater stage, chapel platform |

## 10. Interior walls / surfaces

| Tile | Notes |
|------|-------|
| `Wall` **(existing)** | Generic interior |
| `WallPaneled` | Wood-panel-explosion neighbor home |
| `WallPosters` | Comic shop, kid's room, bait shop interior |
| `WallChalkboard` | Classrooms |
| `WallTiledRestroom` | School / fairground bathrooms |

## 11. Furniture — home

| Tile | Notes |
|------|-------|
| `Furniture` **(existing)** | Generic catch-all (consider splitting below) |
| `Bed` | Bedrooms |
| `Couch` | Living rooms |
| `Chair` | Dining, office |
| `Table` | Dining, kitchen |
| `Dresser` | Bedrooms |
| `Bookshelf` **(existing)** | Living room, study |
| `TvCrt` | Living rooms (electronics era) |
| `ComputerCrt` | Study, school computer labs |
| `Stove` | Kitchens |
| `Fridge` | Kitchens |
| `Sink` | Kitchens, bathrooms |
| `Toilet` | Bathrooms |
| `Bathtub` | Bathrooms |
| `Lamp` | Decoration |
| `Rug` | Living rooms |
| `Plant` **(existing)** | Houseplant decoration |

## 12. Furniture — commercial / fixtures

| Tile | Notes |
|------|-------|
| `Counter` **(existing)** | Generic shop counter |
| `DisplayRackClothes` | Discount + fancy clothes, second hand |
| `DisplayShelfRetail` | Toy store, electronics, bodega, gas station |
| `DisplayCaseGlass` | Pawn shop, comic store cards under lucite, donut case |
| `CashRegister` | Most shops |
| `Booth` | Diner, donut shop |
| `BarStool` | Pub, shabby bar |
| `BarCounter` | Pub, shabby bar |
| `PoolTable` | Shabby bar / pool hall |
| `BowlingPin` | Bowling alley end-cap |
| `MovieScreen` | Theater |
| `TheaterSeat` | Theater |
| `Pew` | Church, chapel |
| `Altar` | Church, chapel |
| `Podium` | Church, town hall |
| `Confessional` | Church |
| `Locker` | School locker rooms, gyms |
| `BoxingRing` | Boxing gym (dominant feature) |
| `WeightRack` | Boxing + fancy gym |
| `TreadmillEra` | Fancy gym aerobics-era equipment |
| `BasketballHoop` | Gyms |
| `VolleyballNet` | Middle school gym |
| `PingPongTable` | Middle school gym |
| `LibraryShelf` | Library |
| `LibraryComputer` | Library new-computer corner |
| `GameTable` | Comic store back tables |
| `TableExamination` | Doctor / dentist |
| `DentalChair` | Dentist |

## 13. Arcade / vending / amusement (game-room flavor)

These deserve their own group — the arcade, movie theater, bowling, gas station, and even bowling alley all reference machines, and they are the heart of the era.

| Tile | Notes |
|------|-------|
| `ArcadeUpright` | Standard upright cabinet |
| `ArcadeSitDown` | Sit-down racer |
| `ArcadeGun` | Light-gun cabinet |
| `ArcadePeriscope` | Periscope cabinet |
| `Pinball` | Misc venues |
| `VendingSnacks` | Snack/drink machine |
| `VendingCigs` | "Oldest smokes vending machine" at the shabby bar |
| `VendingBait` | Bait shop's worm/cricket vending machine (very specific to the area description) |
| `TokenMachine` | Arcade |
| `JukeboxOrSlushie` | Diner / gas station slushie machine |
| `IceCreamCooler` | Bait shop bins (repurposed for live bait) + fast food |

## 14. Outdoor objects & decoration

| Tile | Notes |
|------|-------|
| `Streetlight` | Roads |
| `TrafficLight` | Major intersections |
| `StopSign` | Suburban corners |
| `TelephonePole` | Lining the roads |
| `PowerLine` | Decoration overlay |
| `UtilityBox` | Sidewalks |
| `FireHydrant` | Suburban / downtown |
| `Mailbox` | Suburban |
| `TrashCan` | Parks, sidewalks |
| `Dumpster` | Arcade back, shop alleys |
| `BikeRack` | Arcade ("overflowing"), schools |
| `Bench` | Park, downtown |
| `PicnicTable` | Park, campground, butte |
| `Grill` | Campground, butte |
| `FirePit` | Campground |
| `Hammock` | Campground (broken/remains) |
| `PlaygroundSwing` | Park, school yard |
| `PlaygroundSlide` | Park, school yard |
| `PlaygroundBars` | Park, school yard |
| `Sandbox` | Primary school back |
| `TennisNet` | Park, school |
| `Bleachers` | Dirt track, school field |
| `GoalPost` | High school football field |
| `SoccerGoal` | School field |
| `Tombstone` | Cemetery (consider 2–3 silhouettes: ornate, simple, weathered) |
| `Shrine` | Convent pond's island shrine (called out specifically) |
| `NativityScene` | Meditation center seasonal dressing |
| `ChristmasLights` | Meditation center seasonal |
| `Tent` | Campground |
| `SteamPipe` | Primary school's winter-warmth pipes (specifically called out) |

## 15. Vehicles & rideables (static / parked)

If sprites are reserved for actors, these live in the tile layer as scenery.

| Tile | Notes |
|------|-------|
| `CarParked` | Lots, suburban driveways |
| `TruckParked` | Farm, garage |
| `RvParked` | Fairgrounds RV show |
| `TractorParked` | Farm |
| `Bicycle` | Suburban, school bike rack |
| `BigWheel` | "Steep hill" race callout |
| `DirtBike` | Woods trails |
| `Boat` | Lake (rowboats, kayaks, swan boat — silhouette variants worth it) |
| `BoatCovered` | Tarp-covered rack at boat rental |

## 16. Farm-specific

| Tile | Notes |
|------|-------|
| `Barn` | Farmland |
| `Silo` | Farmland |
| `HayBale` | Farmland |
| `FarmHouse` | Farmland (distinct from suburban silhouette) |
| `Scarecrow` | Field decoration |

## 17. Specialty / one-off

These exist in only one or two areas but are too iconic to fake.

| Tile | Where |
|------|-------|
| `ArcadeNeonAccent` | The angular faded-neon paint on the arcade exterior |
| `BlacktopBlackBox` | Arcade building base color |
| `MeditationCenterFacade` | Stone-and-wood structure built into butte |
| `EventCenterFacade` | Big livestock-smelling barn-shaped event hall |
| `LibraryColumns` | Free-standing classical municipal building |
| `TownHallFacade` | Big formal building |
| `MuseumFacade` | Small but old |
| `Greenhouse` | Glass walls + roof tile pair |
| `SchoolBusStop` | Where farm kids load up |
| `LifeguardChair` | Pool |
| `ConcessionWindow` | Lake clubhouse |

## 18. Per-system rendering notes

A reminder when authoring art for each entry — derived from [project_retrosystems memory](../../../../home/remoteuser/.claude/projects/-usr-src-local-TileEngine-TileEngine/memory/project_retrosystems.md) (verify still accurate against current code):

- **Atari 2600**: 8×8 cells. Lean on silhouette + bold color blocks. `BlackoutWall`, `WindowStainedGlass`, and busy storefronts will need to be heavily abstracted.
- **C64**: 8×8 with 16-color VIC-II palette. Best for distinguishing rooflines and stained glass through hue.
- **Apple II**: 7×8 with artifact colors. Treat colored tiles as approximations.
- **CGA**: 4 colors only — silhouette is everything; many "color-distinct" tiles will collapse to the same look.
- **NES** (default): 16×16 — the reference resolution. Most detail-heavy designs should be sketched here first, then down-rezzed.

## 19. Suggested implementation order

A walkable-prototype path that lets you stand up scenes incrementally:

1. **Core terrain & paths** — `Grass` ✓, `Grass2` ✓, `Dirt`, `Gravel`, `Sidewalk` ✓, `Road` ✓, `Cobblestone`
2. **Water + edges** — `WaterRiver`, `WaterLake`, `WaterEdge`, `BridgeRoad`
3. **Suburban shells** — `HouseVictorian`, `HouseCottage`, `RoofSlate`, `FencePicket`, `Mailbox`
4. **Downtown shells** — `BrickWall`, `WindowStorefront`, `Awning`, `DoorStorefront`
5. **Schoolyard + church set** — `WallStoneTall`, `Steeple`, `WindowStainedGlass`, `Pew`, `Tombstone`, `FenceWroughtIron`
6. **Arcade-and-mall** — `BlackoutWall`, `ArcadeUpright`, `ArcadeSitDown`, `Marquee`, `BowlingLane`
7. **Wilderness** — `TreeDeciduous`, `Boulder`, `WaterCreek`, `Roots`, `PicnicTable`, `FirePit`
8. **Farm + rail** — `PlowedField`, `CornCrop`, `Barn`, `Silo`, `RailroadTrack`, `TrainPlatform`
9. **Interior detail furniture** — split `Furniture` into `Bed`/`Couch`/`Chair`/`Table`/`Dresser`, then layer in `TvCrt`, `Stove`, `Fridge`
10. **Specialty one-offs** — meditation center, event center, museum, lifeguard chair, vending machines, shrine

## 20. Open questions worth resolving before sprite work

- Does **`Furniture`** stay as a catch-all, or should it split into `Bed`/`Couch`/`Chair`/`Table`/`Dresser` now? Splitting earlier saves a migration later but multiplies pixel-art work across 5 systems.
- For **`HouseExterior` variants**: is silhouette-per-style worth the 5× art cost, or should the `Accent` tint plus a single silhouette carry the variation? The current plan assumes silhouette-distinct, but downgrading is cheap.
- **Seasonal variants** (snow, ice, nativity, Christmas lights) — single shared swap, or per-scene?
- **Sprite vs. tile boundary** for parked vehicles, NPCs, and animals — call this out explicitly so a tile isn't authored that should have been a sprite.
