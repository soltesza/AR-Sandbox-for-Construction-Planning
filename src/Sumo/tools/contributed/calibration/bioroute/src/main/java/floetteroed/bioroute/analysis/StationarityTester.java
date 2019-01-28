/*
 * BIOROUTE
 *
 * Copyright 2011-2016 Gunnar Flötteröd and Michel Bierlaire
 * 
 *
 * This file is part of BIOROUTE.
 *
 * BIOROUTE is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * BIOROUTE is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with BIOROUTE.  If not, see <http://www.gnu.org/licenses/>.
 *
 * contact: gunnar.floetteroed@abe.kth.se
 *
 */ 
package floetteroed.bioroute.analysis;

import java.io.IOException;
import java.util.ArrayList;
import java.util.List;

import floetteroed.utilities.math.PolynomialTrendFilter;
import floetteroed.utilities.tabularfileparser.TabularFileHandler;
import floetteroed.utilities.tabularfileparser.TabularFileParser;


/**
 * 
 * @author Gunnar Flötteröd
 * 
 */
class StationarityTester implements TabularFileHandler {

	// -------------------- CONSTANTS --------------------

	private final int regressionWindow;

	private final double slopeThreshold;

	// -------------------- MEMBERS --------------------

	private final List<Integer> binList = new ArrayList<Integer>();

	private final List<Double> similarityList = new ArrayList<Double>();

	// -------------------- CONSTRUCTION --------------------

	StationarityTester(final int regressionWindow, final double slopeThreshold) {
		this.regressionWindow = regressionWindow;
		this.slopeThreshold = slopeThreshold;
	}

	// -------------------- IMPLEMENTATION --------------------

	void run(final String correlationFile) throws IOException {

		final TabularFileParser parser = new TabularFileParser();
		parser.setDelimiterRegex("\\s");
		parser.parse(correlationFile, this);

		for (int startIndex = this.similarityList.size()
				- this.regressionWindow; startIndex >= 0; startIndex--) {

			final PolynomialTrendFilter filter = new PolynomialTrendFilter(1.0,
					1);
			for (int index = startIndex; index < startIndex
					+ this.regressionWindow; index++) {
				filter.add(this.similarityList.get(index));
			}

			final double slope = filter.getRegressionCoefficients().get(1);
			System.out.println(startIndex + "\t" + slope
					+ (Math.abs(slope) <= this.slopeThreshold ? "\tOK" : ""));
		}

	}

	// --------------- IMPLEMENTATION OF TabularFileHandler ---------------

	@Override
	public String preprocess(final String line) {
		return line;
	}

	@Override
	public void startDocument() {
		this.binList.clear();
		this.similarityList.clear();
	}

	@Override
	public void startRow(final String[] row) {
		if (row == null || row.length != 2) {
			System.err.println("skipping row with "
					+ (row == null ? 0 : row.length) + " columns");
		} else {
			this.binList.add(Integer.parseInt(row[0]));
			this.similarityList.add(Double.parseDouble(row[1]));
		}
	}

	@Override
	public void endDocument() {
	}

	public static void main(String[] args) throws IOException {

		System.out.println("STARTED");

		AnalysisRunner.main(new String[] { "STATIONARITY", "-correlationfile",
				"./testdata/corr__psplice_0-5__profract_1-0__mu_0-02.txt",
				"-slopethreshold", "1e-3", "-windowsize", "10" });
	}

}
