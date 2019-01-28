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

import java.io.BufferedReader;
import java.io.File;
import java.io.FileReader;
import java.io.IOException;
import java.io.PrintWriter;
import java.util.logging.Logger;

import floetteroed.cadyts.calibrators.filebased.ChoiceFileWriter;

/**
 * 
 * Writes a new .VEH file.
 * 
 * @author Gunnar Flötteröd
 * 
 */
class DraculaChoiceWriter implements
		ChoiceFileWriter<DraculaAgent, DraculaPlan> {

	// -------------------- MEMBERS --------------------

	private final DraculaCalibrator calibrator;

	private String choiceFile;

	private PrintWriter writer = null;

	private long agentCnt = 0;

	// -------------------- CONSTRUCTION --------------------

	DraculaChoiceWriter(final DraculaCalibrator calibrator) {
		if (calibrator == null) {
			throw new IllegalArgumentException("calibrator is null");
		}
		this.calibrator = calibrator;
	}

	// --------------- IMPLEMENTATION OF ChoiceFileWriter ---------------

	private String tmpFileName() {
		return this.choiceFile + ".TMP";
	}

	@Override
	public void open(String choiceFile) throws IOException {
		this.choiceFile = choiceFile;
		this.writer = new PrintWriter(this.tmpFileName());
		this.agentCnt = 0;
	}

	@Override
	public void write(DraculaAgent agent, DraculaPlan plan) throws IOException {
		this.writer.print(agent.getId());
		this.writer.print("\t");
		// !!! no warmup in .veh file !!!
		this.writer.print(agent.getDepartureTime_s()
				- this.calibrator.getWarmUp_s());
		this.writer.print("\t");
		this.writer.print(plan.getRoute().getId());
		if (agent.getMisc() != null) {
			for (String misc : agent.getMisc()) {
				this.writer.print("\t");
				this.writer.print(misc);
			}
		}
		this.writer.println();
		this.agentCnt++;
	}

	@Override
	public void close() throws IOException {
		this.writer.flush();
		this.writer.close();

		final BufferedReader reader = new BufferedReader(new FileReader(this
				.tmpFileName()));
		this.writer = new PrintWriter(this.choiceFile);
		this.writer.println(this.agentCnt);
		String line;
		while ((line = reader.readLine()) != null) {
			this.writer.println(line);
		}
		this.writer.flush();
		this.writer.close();
		reader.close();

		final boolean tmpDel = (new File(this.tmpFileName())).delete();
		if (!tmpDel) {
			Logger.getLogger(this.getClass().getName()).warning(
					"could not delete temporary file " + this.tmpFileName());
		}
	}

	// -------------------- MAIN FUNCTION, only for testing --------------------

	public static void main(String[] args) throws Exception {

		DraculaCalibrator calibrator = new DraculaCalibrator(null, null, 3600);

		DraculaRoutes routes = new DraculaRoutes(
				"C:\\dracula\\otley\\otley.dem", calibrator);
		System.out.println("LOADED ROUTES FROM .DEM FILE");

		calibrator.setRoutes(routes);

		DraculaTravelTimes travelTimes = new DraculaTravelTimes(
				"C:\\dracula\\otley\\otley.ltt", calibrator);
		System.out.println("LOADED TRAVEL TIMES FROM .LTT FILE");

		calibrator.setTravelTimes(travelTimes);

		DraculaPopulation r = new DraculaPopulation(calibrator);
		System.out.println("LOADED TRAVEL DEMAND FROM .VEH FILE");
		Iterable<DraculaAgent> s = r
				.getPopulationSource("C:\\dracula\\otley\\otley.veh");

		DraculaChoiceWriter w = new DraculaChoiceWriter(calibrator);
		w.open("C:\\dracula\\otley\\otley.veh.test");
		for (DraculaAgent a : s) {
			w.write(a, a.getPlans().get(0));
		}
		w.close();
		System.out.println("WROTE C:\\dracula\\otley\\otley.veh.test");

		System.out.println("DONE");
	}
}
