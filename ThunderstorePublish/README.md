# Enemies Scanner v1.0.3
### Adds shop item that allows to scan the nearby enemies

### Configuration
* Enable scanner ping sound (enable/disable)
* Count of the nearest enemies shown (from 1 to 8)
* Enable/Disable scan filtering by outside enemies
* Cost of the scanner
* Enable/Disable and configure the overheat
* Show/Hide exact distance to the enemy
* Enable/Disable and configure the scanner radius limit
> You can use the Thunderstore to configure the mode. Configuration files are generated after launching the game, with the mod installed, at least once.

### Notes
* The mod is required to be installed on both clients and server
* Some configuration will be synced with server, so configuration of the host will be used (see table below)

| Synced from Server config                   | Client config                         |
|---------------------------------------------|---------------------------------------|
| Shop price                                  | Ping sound                            |
| Overheat (enabled/disabled, time)           | Count of the nearest enemies          |
| Scan radius (enabled/disabled, limit value) | Scan filtering by outside enemies     |
|                                             | Show/Hide exact distance to the enemy |


### Known issues
- Sometimes scanned enemies are not refreshed after going inside and outside the factory. It happens when game doesn't update the corresponding property for scanner object position.
> **Workaround**: change current selected slot to other one and switch back. If it doesn't help try to turn off and drop the scanner, then equip it again. Also, you could enable the 'Disable filtering by outside enemies' feature in the config if needed.

### Credit
New scanner models (v1.0.1+)
* Model by Lord Manok ([3D Artist' Instagram](https://www.instagram.com/lord_manok/))

Legacy scanner model (v.1.0.0)
* Tablet by Poly by Google [CC-BY] via Poly Pizza