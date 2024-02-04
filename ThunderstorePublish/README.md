# Enemies Scanner v1.0.5
### Adds shop item that allows to scan the nearby enemies

### Configuration
`[EnemiesScanner.cfg]`
* Enable scanner ping sound (enable/disable)
* Count of the nearest enemies shown (from 1 to 8)
* Enable/Disable scan filtering by outside enemies
* Cost of the scanner
* Enable/Disable and configure the overheat
* Show/Hide exact distance to the enemy
* Enable/Disable and configure the scanner radius limit
* Battery capacity
* Creatures blacklist (exclude from scanner). Technical creature names list could be found in the [Wiki](https://thunderstore.io/c/lethal-company/p/Kirpichyov/EnemiesScanner/wiki/1444-creature-technical-names/).

`[aliases.json]`
* Creature names aliases
> You can use the Thunderstore to configure the mode. Configuration files are generated after launching the game, with the mod installed, at least once.
> For JSON configs make sure that it's valid after editing and doesn't contain a trailing comma.

### Notes
* The mod is required to be installed on both clients and server
* Some configuration will be synced with server, so configuration of the host will be used (see table below)

| Synced from Server config                   | Client config                         |
|---------------------------------------------|---------------------------------------|
| Shop price                                  | Ping sound                            |
| Overheat (enabled/disabled, time)           | Count of the nearest enemies          |
| Scan radius (enabled/disabled, limit value) | Scan filtering by outside enemies     |
| Battery capacity                            | Show/Hide exact distance to the enemy |
| Creatures blacklist                         | Creature names aliases                |

### Known issues
- Sometimes scanned enemies are not refreshed after going inside and outside the factory. It happens when game doesn't update the corresponding property for scanner object position.
> **Workaround**: change current selected slot to other one and switch back. If it doesn't help try to turn off and drop the scanner, then equip it again. Also, you could enable the 'Disable filtering by outside enemies' feature in the config if needed.

### Credit
New scanner models (v1.0.1+)
* Model by Lord Manok ([3D Artist' Instagram](https://www.instagram.com/lord_manok/))

Legacy scanner model (v.1.0.0)
* Tablet by Poly by Google [CC-BY] via Poly Pizza