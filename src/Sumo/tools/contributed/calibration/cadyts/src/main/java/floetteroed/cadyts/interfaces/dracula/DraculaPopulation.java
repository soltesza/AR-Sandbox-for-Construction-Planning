/*
 * Cadyts - Calibration of dynamic traffic simulations
 *
 * Copyright 2009-2016 Gunnar Flötteröd
 * 
 *
 * This file is part of Cadyts.
 *
 * Cadyts is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * Cadyts is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Cadyts.  If not, see <http://www.gnu.org/licenses/>.
 *
 * contact: gunnar.floetteroed@abe.kth.se
 *
 */ 
package floetteroed.cadyts.interfaces.dracula;

import static java.lang.Double.parseDouble;
import static java.lang.Math.round;
import static java.util.Arrays.copyOfRange;

import java.io.IOException;
import java.util.LinkedHashMap;
import java.util.Map;

import floetteroed.cadyts.calibrators.filebased.PopulationFileReader;
import floetteroed.cadyts.demand.PlanChoiceModel;
import floetteroed.utilities.tabularfileparser.TabularFileHandler;
import floetteroed.utilities.tabularfileparser.TabularFileParser;

/**
 * 
 * Loads a population of vehicles from the .VEH file.
 * 
 * @author Gunnar Flötteröd
 * 
 */
class DraculaPopulation implements TabularFileHandler,
		PopulationFileReader<DraculaAgent> {

	// -------------------- CONSTANTS --------------------

	private final static int ID_INDEX = 0;

	private final static int DPT_TIME_INDEX = 1;

	private final static int ROUTE_INDEX = 2;

	private final static int MISC_START_INDEX = 3;

	// -------------------- MEMBERS --------------------

	private final DraculaCalibrator calibrator;

	private final Map<Long, DraculaAgent> id2agent = new LinkedHashMap<Long, DraculaAgent>();

	// -------------------- CONSTRUCTION --------------------

	DraculaPopulation(final DraculaCalibrator calibrator) {
		if (calibrator == null) {
			throw new IllegalArgumentException("calibrator is null");
		}
		this.calibrator = calibrator;
	}

	// --------------- IMPLEMENTATION OF PopulationFileReader ---------------

	@Override
	public Iterable<DraculaAgent> getPopulationSource(
			final String populationFile) {
		this.id2agent.clear();
		final TabularFileParser parser = new TabularFileParser();
		parser.setDelimiterRegex("\\s");
		parser.setMinRowLength(MISC_START_INDEX);
		try {
			parser.parse(populationFile, this);
		} catch (IOException e) {
			e.printStackTrace();
		}
		return this.id2agent.values();
	}

	// --------------- IMPLEMENTATION OF TabularFileHandler ---------------

	@Override
	public void startDocument() {
	}

	@Override
	public void endDocument() {
	}

	@Override
	public String preprocess(String line) {
		return line;
	}
	
	@Override
	public void startRow(String[] row) {
		/*
		 * (1) extract relevant file entries
		 */
		final long agentId = Long.parseLong(row[ID_INDEX]);
		final int dptTime_s = this.calibrator.getWarmUp_s()
				+ (int) round(parseDouble(row[DPT_TIME_INDEX]));
		// !!! no warmup in .veh file !!!
		final long routeId = Long.parseLong(row[ROUTE_INDEX]);
		final String[] misc = copyOfRange(row, MISC_START_INDEX, row.length);
		/*
		 * (2) identify according data structures
		 */
		final DraculaODRelation od = this.calibrator.getRoutes().getRoute(
				routeId).getOD();
		final PlanChoiceModel<DraculaPlan> choiceModel = new DraculaPlanChoiceModel(
				this.calibrator);
		final DraculaAgent agent = new DraculaAgent(agentId, choiceModel, od,
				dptTime_s, misc);
		/*
		 * (3) provide all real plans (those which imply physical movement)
		 */
		for (DraculaRoute route : agent.getOD().getRoutes()) {
			final DraculaPlan plan = new DraculaPlan(route, dptTime_s,
					this.calibrator.newPlanBuilder(), this.calibrator
							.getTravelTimes());
			agent.addPlan(plan);
		}
		/*
		 * (4) provide stay-at-home plan as last alternative
		 */
		final DraculaRoute stayAtHomeRoute = new DraculaRoute(0L, od);
		final DraculaPlan stayAtHomePlan = new DraculaPlan(stayAtHomeRoute,
				dptTime_s, this.calibrator.newPlanBuilder(), null);
		agent.addPlan(stayAtHomePlan);
		/*
		 * (5) store complete agent
		 */
		this.id2agent.put(agentId, agent);
	}

	// -------------------- MAIN-FUNCTION, ONLY FOR TESTING --------------------

	public static void main(String[] args) {

		DraculaCalibrator calibrator = new DraculaCalibrator(null, null, 3600);

		DraculaRoutes routes = new DraculaRoutes("C:\\dracula\\net1\\net1.dem",
				calibrator);
		System.out.println("LOADED ROUTES FROM .DEM FILE");

		calibrator.setRoutes(routes);

		DraculaTravelTimes travelTimes = new DraculaTravelTimes(
				"C:\\dracula\\net1\\net1.ltt", calibrator);
		calibrator.setTravelTimes(travelTimes);
		System.out.println("LOADED TRAVEL TIMES FROM .LTT FILE");

		DraculaPopulation r = new DraculaPopulation(calibrator);
		for (DraculaAgent agent : r
				.getPopulationSource("C:\\dracula\\net1\\net1-initial.veh")) {
			System.out.println(agent);
		}
		System.out.println("LOADED TRAVEL DEMAND FROM .VEH FILE");
		System.out.println("DONE");
	}
}
