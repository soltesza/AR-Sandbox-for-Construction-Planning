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

import java.io.File;
import java.io.FileNotFoundException;
import java.io.PrintWriter;

import floetteroed.opdyts.DecisionVariable;

/**
 * 
 * @author Gunnar Flötteröd
 *
 */
public class FileBasedDecisionVariable implements DecisionVariable {

	// -------------------- CONSTANTS --------------------

	private final String decisionVariableId;

	private final String executionFolder;

	private final String newDecisionVariableFileName;

	// -------------------- CONSTRUCTION --------------------

	public FileBasedDecisionVariable(final String decisionVariableId, final String executionFolder,
			final String decisionVariableFileName) {
		this.decisionVariableId = decisionVariableId;
		this.executionFolder = executionFolder;
		this.newDecisionVariableFileName = decisionVariableFileName;
	}

	// -------------------- FILE-BASED FUNCTIONALITY --------------------

	public void writeToNewDecisionVariableFile(final String folder, final String fileName) {
		try {
			final PrintWriter writer = new PrintWriter(new File(folder, fileName));
			writer.print(this.decisionVariableId);
			writer.flush();
			writer.close();
		} catch (FileNotFoundException e) {
			throw new RuntimeException(e);
		}
	}

	// --------------- IMPLEMENTATION OF DecisionVariable ---------------

	@Override
	public void implementInSimulation() {
		this.writeToNewDecisionVariableFile(this.executionFolder, this.newDecisionVariableFileName);
	}

	// -------------------- OVERRIDING OF Object --------------------

	@Override
	public String toString() {
		return this.decisionVariableId;
	}
}
