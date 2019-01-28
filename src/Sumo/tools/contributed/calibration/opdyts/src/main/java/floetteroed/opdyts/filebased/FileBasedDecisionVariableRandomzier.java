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
import java.util.Collection;
import java.util.LinkedList;
import java.util.List;

import floetteroed.opdyts.DecisionVariableRandomizer;

/**
 * 
 * @author Gunnar Flötteröd
 *
 */
public class FileBasedDecisionVariableRandomzier implements DecisionVariableRandomizer<FileBasedDecisionVariable> {

	// -------------------- CONSTANTS --------------------

	private final String executionFolder;

	private final String createNewDecisionVariablesCommand;

	private final String originalDecisionVariableFileName;

	private final String newDecisionVariablesFileName;

	// -------------------- CONSTRUCTION --------------------

	public FileBasedDecisionVariableRandomzier(final String executionFolder,
			final String createNewDecisionVariablesCommand, final String originalDecisionVariableFileName,
			final String newDecisionVariablesFileName) {
		this.executionFolder = executionFolder;
		this.createNewDecisionVariablesCommand = createNewDecisionVariablesCommand;
		this.originalDecisionVariableFileName = originalDecisionVariableFileName;
		this.newDecisionVariablesFileName = newDecisionVariablesFileName;
	}

	// ---------- IMPLEMENTATION OF DecisionVariableRandomizer ----------

	@Override
	public Collection<FileBasedDecisionVariable> newRandomVariations(
			FileBasedDecisionVariable originalDecisionVariable) {

		if (originalDecisionVariable != null) {
			originalDecisionVariable.writeToNewDecisionVariableFile(this.executionFolder,
					this.originalDecisionVariableFileName);
		} else {
//			final File file = new File(this.executionFolder, this.originalDecisionVariableFileName);
//			if (file.exists()) {
//				file.delete();
//			}
		}

		final Process proc;
		final int exitVal;
		try {
			proc = Runtime.getRuntime().exec(this.createNewDecisionVariablesCommand, null,
					new File(this.executionFolder));
			exitVal = proc.waitFor();
			if (exitVal != 0) {
				throw new RuntimeException("Decision variable generation terminated with exit code " + exitVal + ".");
			}
		} catch (Exception e) {
			throw new RuntimeException(e);
		}

		final List<FileBasedDecisionVariable> result = new LinkedList<>();
		try {
			String line;
			final BufferedReader reader = new BufferedReader(
					new FileReader(new File(this.executionFolder, this.newDecisionVariablesFileName)));
			while ((line = reader.readLine()) != null) {
				result.add(new FileBasedDecisionVariable(line.trim(), this.executionFolder,
						this.originalDecisionVariableFileName));
			}
			reader.close();
		} catch (Exception e) {
			throw new RuntimeException(e);
		}
		return result;
	}
}
