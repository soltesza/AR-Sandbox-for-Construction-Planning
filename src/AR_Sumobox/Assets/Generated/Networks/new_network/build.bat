python "%SUMO_HOME%\tools\ptlines2flows.py" -n osm.net.xml -e 3600 -p 600 --random-begin --seed 42 --ptstops osm_stops.add.xml --ptlines osm_ptlines.xml -o osm_pt.rou.xml --ignore-errors --vtype-prefix pt_ --stopinfos-file stopinfos.xml --routes-file vehroutes.xml --trips-file trips.trips.xml --verbose
python "%SUMO_HOME%\tools\randomTrips.py" -n osm.net.xml --seed 42 --fringe-factor 1 -p 3.152379 -r osm.pedestrian.rou.xml -o osm.pedestrian.trips.xml -e 3600 --vehicle-class pedestrian --persontrips --prefix ped --trip-attributes "modes=\"public\"" --additional-files osm_stops.add.xml,osm_pt.rou.xml
python "%SUMO_HOME%\tools\randomTrips.py" -n osm.net.xml --seed 42 --fringe-factor 2 -p 5.614748 -r osm.bicycle.rou.xml -o osm.bicycle.trips.xml -e 3600 --vehicle-class bicycle --vclass bicycle --prefix bike --fringe-start-attributes "departSpeed=\"max\"" --max-distance 8000 --trip-attributes "departLane=\"best\"" --validate
python "%SUMO_HOME%\tools\randomTrips.py" -n osm.net.xml --seed 42 --fringe-factor 2 -p 9.027167 -r osm.motorcycle.rou.xml -o osm.motorcycle.trips.xml -e 3600 --vehicle-class motorcycle --vclass motorcycle --prefix moto --fringe-start-attributes "departSpeed=\"max\"" --max-distance 1200 --trip-attributes "departLane=\"best\"" --validate
python "%SUMO_HOME%\tools\randomTrips.py" -n osm.net.xml --seed 42 --fringe-factor 5 -p 3.009056 -r osm.passenger.rou.xml -o osm.passenger.trips.xml -e 3600 --vehicle-class passenger --vclass passenger --prefix veh --min-distance 300 --trip-attributes "departLane=\"best\"" --fringe-start-attributes "departSpeed=\"max\"" --allow-fringe.min-length 1000 --lanes --validate
python "%SUMO_HOME%\tools\randomTrips.py" -n osm.net.xml --seed 42 --fringe-factor 5 -p 4.513584 -r osm.truck.rou.xml -o osm.truck.trips.xml -e 3600 --vehicle-class truck --vclass truck --prefix truck --min-distance 600 --fringe-start-attributes "departSpeed=\"max\"" --trip-attributes "departLane=\"best\"" --validate
python "%SUMO_HOME%\tools\randomTrips.py" -n osm.net.xml --seed 42 --fringe-factor 5 -p 9.027167 -r osm.bus.rou.xml -o osm.bus.trips.xml -e 3600 --vehicle-class bus --vclass bus --prefix bus --min-distance 600 --fringe-start-attributes "departSpeed=\"max\"" --trip-attributes "departLane=\"best\"" --validate
