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

import static floetteroed.bioroute.BiorouteRunner.BIOROUTE_ELEMENT;
import static floetteroed.bioroute.BiorouteRunner.CLASSNAME_ELEMENT;
import static floetteroed.bioroute.BiorouteRunner.NETWORKLOADER_ELEMENT;
import static floetteroed.bioroute.BiorouteRunner.NETWORKPREPROCESSOR_ELEMENT;
import static floetteroed.bioroute.BiorouteRunner.PATHWRITER_CONFIG_ELEMENT;
import static floetteroed.bioroute.pathwriter.PathXMLWriter.FILENAME_ELEMENT;

import java.io.BufferedReader;
import java.io.FileReader;
import java.io.IOException;

import floetteroed.bioroute.utilities.NetworkInstantiator;
import floetteroed.utilities.ErrorMsgPrinter;
import floetteroed.utilities.commandlineparser.CommandLineParser;
import floetteroed.utilities.commandlineparser.CommandLineParserElement;
import floetteroed.utilities.config.Config;
import floetteroed.utilities.config.ConfigReader;
import floetteroed.utilities.networks.basic.BasicNetwork;

/**
 * Provides various analysis functionality for sampled path sets.
 * 
 * @author Gunnar Flötteröd
 * 
 */
public class AnalysisRunner {

	// -------------------- CONSTANTS --------------------

	public static final String CONFIGFILE_KEY = "-configfile";
	public static final String RESULTFILE_KEY = "-resultfile";

	// -------------------- CONSTRUCTION --------------------

	protected AnalysisRunner() {
	}

	// -------------------- STATIONARITY ANALYSIS --------------------

	public static final String STATIONARITY_MAINKEY = "STATIONARITY";
	public static final String CORRELATIONFILE_KEY = "-correlationfile";
	public static final String WINDOWSIZE_KEY = "-windowsize";
	public static final String SLOPETHRESHOLD_KEY = "-slopethreshold";

	private void stationarityAnalysis(final String[] params) throws IOException {
		/*
		 * (1) extract and check command line parameters
		 */
		final CommandLineParser clp = new CommandLineParser();
		clp.defineParameter(CORRELATIONFILE_KEY, true, null,
				"result file of a CORRELATION analysis");
		// clp.defineParameter(RESULTFILE_KEY, true, null,
		// "the file where to write the results");
		clp.defineParameter(WINDOWSIZE_KEY, false, Integer.toString(10),
				"regression window size");
		clp.defineParameter(SLOPETHRESHOLD_KEY, false, Double.toString(1e-3),
				"stationarity slope threshold");
		clp.parse(params);
		if (!clp.isComplete()) {
			System.out.println(STATIONARITY_MAINKEY
					+ " takes the following parameters:");
			for (CommandLineParserElement element : clp.getElements()) {
				System.out.println("  " + element.toString());
			}
			System.exit(-1);
		}

		/*
		 * (2) run the analysis algorithm
		 */
		final StationarityTester st = new StationarityTester(
				clp.getInteger(WINDOWSIZE_KEY),
				clp.getDouble(SLOPETHRESHOLD_KEY));
		st.run(clp.getString(CORRELATIONFILE_KEY));
	}

	// -------------------- FREQUENCY ANALYSIS --------------------

	public static final String FREQUENCIES_MAINKEY = "FREQUENCIES";
	public static final String SAMPLEINTERVAL_KEY = "-sampleinterval";
	public static final String TOTALKS_KEY = "-totals";

	private void frequenciesAnalysis(final String[] params) throws IOException {
		/*
		 * (1) extract and check command line parameters
		 */
		final CommandLineParser clp = new CommandLineParser();
		clp.defineParameter(CONFIGFILE_KEY, true, null,
				"the configuration file");
		clp.defineParameter(RESULTFILE_KEY, true, null,
				"the file where to write the results");
		clp.defineParameter(SAMPLEINTERVAL_KEY, false, Integer.toString(1),
				"interval at which paths are taken from file");
		clp.defineParameter(TOTALKS_KEY, false, "false",
				"if totals instead of frequencies are to be computed");
		clp.parse(params);
		if (!clp.isComplete()) {
			System.out.println(FREQUENCIES_MAINKEY
					+ " takes the following parameters:");
			for (CommandLineParserElement element : clp.getElements()) {
				System.out.println("  " + element.toString());
			}
			System.exit(-1);
		}
		/*
		 * (2) build data structures
		 */
		final String configFile = clp.getString(CONFIGFILE_KEY);
		final Config config = (new ConfigReader()).read(configFile);
		final String pathfile = config.absolutePath(config.get(
				BIOROUTE_ELEMENT, PATHWRITER_CONFIG_ELEMENT, FILENAME_ELEMENT));
		final BasicNetwork network = NetworkInstantiator.loadNetwork(config,
				BIOROUTE_ELEMENT, NETWORKLOADER_ELEMENT, CLASSNAME_ELEMENT);
		NetworkInstantiator.preprocessNetwork(network, config,
				BIOROUTE_ELEMENT, NETWORKPREPROCESSOR_ELEMENT,
				CLASSNAME_ELEMENT);
		/*
		 * (3) run the analysis algorithm
		 */
		final FrequencyAnalyzer sa = new FrequencyAnalyzer(network,
				clp.getInteger(SAMPLEINTERVAL_KEY), clp.getBoolean(TOTALKS_KEY));
		sa.run(pathfile, clp.getString(RESULTFILE_KEY));
	}

	// -------------------- CHI^2 ANALYSIS --------------------

	public static final String CHI2_MAINKEY = "CHI2";
	public static final String TOTALSFILE_KEY = "-totalsfile";

	// this is public such that it can accessed by bioroute unit tests
	public double computeChi2(final String totalsFile, final boolean msg)
			throws IOException {
		final BufferedReader reader = new BufferedReader(new FileReader(
				totalsFile));
		double chi2 = 0;
		int distinct = 0;
		int total = 0;
		String line;
		do {
			line = reader.readLine();
			if (line != null && !"".equals(line.trim())) {
				final String[] columns = line.split("\\s");
				final double e = Double.parseDouble(columns[0].trim());
				final double o = Double.parseDouble(columns[1].trim());
				chi2 += ((o - e) / e) * (o - e);
				distinct++;
				total += (int) o;
			} else {
				if (total > 0 && msg) {
					System.out.println("chi2 = " + chi2 + "; computed from "
							+ distinct + "(" + total
							+ ") distinct(total) observations");
				}
				// chi2 = 0;
				// distinct = 0;
				// total = 0;
			}
		} while (line != null);
		reader.close();
		return chi2;
	}

	private void chi2Analysis(final String[] params) throws IOException {
		/*
		 * (1) extract and check command line parameters
		 */
		final CommandLineParser clp = new CommandLineParser();
		clp.defineParameter(TOTALSFILE_KEY, true, null,
				"the totals file to be analyzed");
		clp.parse(params);
		if (!clp.isComplete()) {
			System.out.println(CHI2_MAINKEY
					+ " takes the following parameters:");
			for (CommandLineParserElement element : clp.getElements()) {
				System.out.println("  " + element.toString());
			}
			System.exit(-1);
		}
		/*
		 * (2) build data structures
		 */
		final String totalsFile = clp.getString(TOTALSFILE_KEY);
		/*
		 * (3) run the analysis algorithm
		 */
		computeChi2(totalsFile, true);
		// TODO NEW
		// final BufferedReader reader = new BufferedReader(new FileReader(
		// totalsFile));
		// double chi2 = 0;
		// int distinct = 0;
		// int total = 0;
		// String line;
		// do {
		// line = reader.readLine();
		// if (line != null && !"".equals(line.trim())) {
		// final String[] columns = line.split("\\s");
		// final double e = Double.parseDouble(columns[0].trim());
		// final double o = Double.parseDouble(columns[1].trim());
		// chi2 += ((o - e) / e) * (o - e);
		// distinct++;
		// total += (int) o;
		// } else {
		// if (total > 0) {
		// System.out.println("chi2 = " + chi2 + "; computed from "
		// + distinct + "(" + total
		// + ") distinct(total) observations");
		// }
		// chi2 = 0;
		// distinct = 0;
		// total = 0;
		// }
		// } while (line != null);
		// reader.close();
	}

	// -------------------- CORRELATION ANALYSIS --------------------

	public static final String CORRELATION_MAINKEY = "CORRELATION";
	// public static final String DISTANCEBIN_KEY = "-distancebin";
	public static final String MAXDISTANCE_KEY = "-maxdistance";

	private void correlationAnalysis(final String[] params) throws IOException {
		/*
		 * (1) define, extract and check command line parameters
		 */
		final CommandLineParser clp = new CommandLineParser();
		clp.defineParameter(CONFIGFILE_KEY, true, null,
				"the configuration file");
		// clp.defineParameter(DISTANCEBIN_KEY, false, "1",
		// "at which discretization distance in the chain is evaluated");
		clp.defineParameter(MAXDISTANCE_KEY, true, null,
				"the largest analyzed distance in the chain");
		clp.defineParameter(RESULTFILE_KEY, true, null,
				"the file where to write the results");
		clp.parse(params);
		if (!clp.isComplete()) {
			System.out.println(CORRELATION_MAINKEY
					+ " takes the following parameters:");
			for (CommandLineParserElement element : clp.getElements()) {
				System.out.println("  " + element.toString());
			}
			System.exit(-1);
		}
		/*
		 * (2) build data structures
		 */
		final Config config = (new ConfigReader()).read(clp
				.getString(CONFIGFILE_KEY));
		final String pathfile = config.absolutePath(config.get(
				BIOROUTE_ELEMENT, PATHWRITER_CONFIG_ELEMENT, FILENAME_ELEMENT));
		final BasicNetwork network = NetworkInstantiator.loadNetwork(config,
				BIOROUTE_ELEMENT, NETWORKLOADER_ELEMENT, CLASSNAME_ELEMENT);
		NetworkInstantiator.preprocessNetwork(network, config,
				BIOROUTE_ELEMENT, NETWORKPREPROCESSOR_ELEMENT,
				CLASSNAME_ELEMENT);
		/*
		 * (3) run the analysis algorithm
		 */
		final SimilarityAnalyzer ca = new SimilarityAnalyzer(
				clp.getInteger(MAXDISTANCE_KEY), // clp.getInteger(DISTANCEBIN_KEY),
				network);
		ca.run(pathfile, clp.getString(RESULTFILE_KEY));
	}

	// -------------------- VISUAL ANALYSIS --------------------

	private static final String VISUAL_MAINKEY = "VISUAL";
	private static final String VISCONFIGFILE_KEY = "-visconfigfile";
	private static final String VISDATAFILE_KEY = "-visdatafile";

	private void visualAnalysis(final String[] params) throws IOException {
		/*
		 * (1) extract and check command line parameters
		 */
		final CommandLineParser clp = new CommandLineParser();
		clp.defineParameter(CONFIGFILE_KEY, true, null,
				"the configuration file");
		clp.defineParameter(VISCONFIGFILE_KEY, true, null,
				"the file where to write the visual configuration");
		clp.defineParameter(VISDATAFILE_KEY, true, null,
				"the file where to write the visualization data");
		clp.parse(params);
		if (params.length == 0 || !clp.isComplete()) {
			System.out.println(VISUAL_MAINKEY
					+ " takes the following parameters:");
			for (CommandLineParserElement element : clp.getElements()) {
				System.out.println("  " + element.toString());
			}
			System.exit(-1);
		}
		/*
		 * (2) build data structures
		 */
		final Config config = (new ConfigReader()).read(clp
				.getString(CONFIGFILE_KEY));
		final BasicNetwork network = NetworkInstantiator.loadNetwork(config,
				BIOROUTE_ELEMENT, NETWORKLOADER_ELEMENT, CLASSNAME_ELEMENT);
		NetworkInstantiator.preprocessNetwork(network, config,
				BIOROUTE_ELEMENT, NETWORKPREPROCESSOR_ELEMENT,
				CLASSNAME_ELEMENT);
		final String pathFile = config.absolutePath(config.get(
				BIOROUTE_ELEMENT, PATHWRITER_CONFIG_ELEMENT, FILENAME_ELEMENT));
		/*
		 * (3) run the analysis algorithm
		 */
		final VisualAnalyzer va = newVisualAnalyzer(config, network);
		va.run(pathFile, clp.getString(VISCONFIGFILE_KEY),
				clp.getString(VISDATAFILE_KEY));
	}

	protected VisualAnalyzer newVisualAnalyzer(final Config config,
			final BasicNetwork network) {
		return new VisualAnalyzer(network, config.newSubConfig(
				BIOROUTE_ELEMENT, NETWORKLOADER_ELEMENT));
	}

	// -------------------- RUNNER IMPLEMENTATION --------------------

	private void taskInfo() {
		System.out.println("First argument must be one of the following:");
		System.out.println("  " + STATIONARITY_MAINKEY
				+ " --> for stationarity analysis");
		System.out.println("  " + CORRELATION_MAINKEY
				+ " --> for within-chain correlation analysis");
		System.out.println("  " + FREQUENCIES_MAINKEY
				+ " --> for sample frequency analysis");
		System.out.println("  " + CHI2_MAINKEY + " --> for chi2 analysis");
		System.out.println("  " + VISUAL_MAINKEY + " --> for visual analysis");
	}

	public void run(final String[] args) {
		try {

			System.out.println("STARTED..");
			/*
			 * CHECK COMMAND LINE ARGUMENTS FOR BASIC FEASIBILIY
			 */
			if (args == null || args.length < 1) {
				this.taskInfo();
				System.exit(-1);
			}
			/*
			 * DECIDE WHAT TO DO
			 */
			final String task = args[0].toUpperCase();
			final String[] params = new String[args.length - 1];
			System.arraycopy(args, 1, params, 0, args.length - 1);
			if (STATIONARITY_MAINKEY.equals(task)) {
				// TODO NEW
				System.out.println("Starting stationarity analysis.");
				this.stationarityAnalysis(params);
			} else if (CORRELATION_MAINKEY.equals(task)) {
				System.out.println("Starting correlation analysis.");
				this.correlationAnalysis(params);
			} else if (FREQUENCIES_MAINKEY.equals(task)) {
				System.out.println("Starting frequency analysis.");
				this.frequenciesAnalysis(params);
			} else if (CHI2_MAINKEY.equals(task)) {
				System.out.println("Starting chi2 analysis.");
				this.chi2Analysis(params);
			} else if (VISUAL_MAINKEY.equals(task)) {
				System.out.println("Starting visual analysis.");
				this.visualAnalysis(params);
			} else {
				System.out.println("Unknown analysis task: " + task);
				this.taskInfo();
				System.exit(-1);
			}
			System.out.println("..DONE");

		} catch (Exception e) {
			ErrorMsgPrinter.toStdOut(e);
			ErrorMsgPrinter.toErrOut(e);
		}
	}

	public static void main(String[] args) {
		final AnalysisRunner runner = new AnalysisRunner();
		runner.run(args);
	}
}
