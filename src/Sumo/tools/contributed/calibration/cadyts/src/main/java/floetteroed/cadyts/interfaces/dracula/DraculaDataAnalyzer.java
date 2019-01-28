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

import java.io.File;
import java.util.ArrayList;
import java.util.LinkedHashSet;
import java.util.List;
import java.util.Set;

import floetteroed.utilities.DynamicData;



/**
 * 
 * @author Gunnar Flötteröd
 * 
 */
class DraculaDataAnalyzer {

	// -------------------- MEMBERS --------------------

	private final Set<String> allKeys = new LinkedHashSet<String>();

	private final List<DynamicData<String>> dataList = new ArrayList<DynamicData<String>>();

	// -------------------- CONSTRUCTION --------------------

	DraculaDataAnalyzer(final String prefix) {
		final DynamicXMLFileIOStringKey reader = new DynamicXMLFileIOStringKey();
		int i = 0;
		File f;
		while ((f = new File(prefix + i + ".xml")).exists()) {
			final DynamicData<String> data = reader.read(f.getAbsolutePath());
			this.allKeys.addAll(data.keySet());
			this.dataList.add(data);
			i++;
		}
		System.out.println(allKeys);
	}

	// -------------------- GETTERS --------------------

	int getEntries() {
		return dataList.size();
	}

	// -------------------- IMPLEMENTATION --------------------

	void printDataField(final String key) {
		final StringBuffer result = new StringBuffer();
		for (int i = 0; i < this.getEntries(); i++) {
			DynamicData<String> data = this.dataList.get(i);
			for (int bin = 0; bin < data.getBinCnt(); bin++) {
				result.append(data.getBinValue(key, bin));
				result.append("\t");
			}
			result.append("\n");
		}
		System.out.println();
		System.out.println(result);
		System.out.println();
	}

	public static final void main(String[] args) {

		// final String path =
		// "C:\\Documents and Settings\\floetter\\My Documents\\temp\\Leeds_2010\\net1\\";
		final String path = "C:\\dracula\\net1\\";
		final String link = "3"; // "9";

		DraculaDataAnalyzer dda = new DraculaDataAnalyzer(path + "flows");
		System.out.println("found " + dda.getEntries() + " entries");
		dda.printDataField(link);
		System.out.println();

		dda = new DraculaDataAnalyzer(path + "tt");
		System.out.println("found " + dda.getEntries() + " entries");
		dda.printDataField(link);

		System.out.println("DONE");
	}
}
