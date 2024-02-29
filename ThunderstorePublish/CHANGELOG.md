## v1.0.0
- Initial public version

## v.1.0.1
- Updated scanner item model
- Fixed a bug when pocketed scanner lights weren't hidden

## v.1.0.2
- Excluded 2 harmless mobs from scanner entries (Roaming Locust and Manticoil)
- Now scan records sorted ascending not descending, so the nearest entry comes first
- Replaced danger sound
- Added warning sound
- Added ability to configure the scanner
- Warning trigger adjusted further by 2 meters

## v.1.0.3
- Added new improved scanner model by Lord Manok
- Fixed a bug when scanner doesn't show nearest enemies and shows the furthest instead when enemies count are greater than configured limit
- Added ability to configure the cost of the scanner
- Added overheat feature along with configuration
- Added ability to configure if the exact distance to the enemy should be hidden or shown
- Added ability to configure the scanner radius
- 'GG' label replaced with 'LETHAL'

## v.1.0.4
- Fixed a bug when scanner lights don't turn off completely when the scanner is pocketed ([GitHub Issue](https://github.com/kirpichyov/LC_EnemiesScanner/issues/5))
- Added ability to configure the battery capacity of the scanner
- Updated sounds (replaced equip, pocketed, turn on and turn off sounds)
- Added run-out-of-battery sound

## v.1.0.5
Implement some of the suggestions from [GitHub Issue](https://github.com/kirpichyov/LC_EnemiesScanner/issues/7):
- Reduced the model size by 18%
- Moved heat indicator to the top right
- Added ability to configure the scanner blacklist that allows to exclude creatures from scanner (Turret now excluded by default)
- Added ability to configure the aliases for creature names in the scanner (Now creature names from bestiary are used instead of technical names by default)

## v.1.0.6
- Added ability to blacklist the creatures from the mods [GitHub Issue](https://github.com/kirpichyov/LC_EnemiesScanner/issues/9)
- Added console log for creatures from mods with information that includes names to exclude it from scanner or set an alias (see Wiki for more information)
- Added experimental config option that allows to set the scanner refresh rate