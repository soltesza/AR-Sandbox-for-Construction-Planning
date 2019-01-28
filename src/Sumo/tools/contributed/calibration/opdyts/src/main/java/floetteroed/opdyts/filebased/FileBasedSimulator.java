/*
 * Opdyts - Optimization of dynamic traffic simulations
 *
 * Copyright 2015, 2016 Gunnar Flötteröd
 * 
 *
 * This file is part of Opdyts.
 *
 * Opdyts is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * Opdyts is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Opdyts.  If not, see <http://www.gnu.org/licenses/>.
 *
 * contact: gunnar.floetteroed@abe.kth.se
 *
 */
package floetteroed.opdyts.filebased;

import java.io.BufferedReader;
import java.io.File;
import java.io.FileReader;
import java.util.LinkedList;
import java.util.List;

import floetteroed.opdyts.SimulatorState;
import floetteroed.opdyts.searchalgorithms.Simulator;
import floetteroed.opdyts.trajectorysampling.TrajectorySampler;
import floetteroed.utilities.math.Vector;

/**
 * 
 * @author Gunnar Flötteröd
 *
 */
public class FileBasedSimulator implements Simulator<FileBasedDecisionVariable> {

	// -------------------- CONSTANTS --------------------

	private final String executionFolder;

	private final String advanceSimulationCommand;

	private final String newStateFileName;

	// -------------------- CONSTRUCTION --------------------

	public FileBasedSimulator(final String executionFolder, final String advanceSimulationCommand,
			final String newStateFileName) {
		this.executionFolder = executionFolder;
		this.advanceSimulationCommand = advanceSimulationCommand;
		this.newStateFileName = newStateFileName;
	}

	// -------------------- INTERNALS --------------------

	private void advanceSimulation() {
		final Process proc;
		final int exitVal;
		try {
			proc = Runtime.getRuntime().exec(this.advanceSimulationCommand, null, new File(this.executionFolder));
			exitVal = proc.waitFor();
			if (exitVal != 0) {
				throw new RuntimeException("Simulation terminated with exit code " + exitVal + ".");
			}
		} catch (Exception e) {
			throw new RuntimeException(e);
		}
	}

	private FileBasedSimulatorState loadNewState() {
		final List<Double> numbers = new LinkedList<>();
		try {
			String line;
			final BufferedReader reader = new BufferedReader(
					new FileReader(new File(this.executionFolder, this.newStateFileName)));
			while ((line = reader.readLine()) != null) {
				line = line.trim();
				numbers.add(Double.parseDouble(line));
			}
			reader.close();
		} catch (Exception e) {
			throw new RuntimeException(e);
		}
		return new FileBasedSimulatorState(numbers.get(0), new Vector(numbers.subList(1, numbers.size())));
	}

	// -------------------- IMPLEMENTATION OF Simulator --------------------

	@Override
	public SimulatorState run(final TrajectorySampler<FileBasedDecisionVariable> evaluator) {
		return this.run(evaluator, null);
	}

	@Override
	public SimulatorState run(final TrajectorySampler<FileBasedDecisionVariable> evaluator,
			final SimulatorState initialState) {
		// evaluator.initialize();
		FileBasedSimulatorState newState = null;
		while (!evaluator.foundSolution()) {
			this.advanceSimulation();
			newState = this.loadNewState();
			evaluator.afterIteration(newState);
		}
		return newState;
	}
}
