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

import java.io.FileNotFoundException;
import java.io.IOException;
import java.io.PrintWriter;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import org.xml.sax.Attributes;

import floetteroed.bioroute.pathgenerator.metropolishastings.MHPathWriterWrapper;
import floetteroed.utilities.networks.basic.BasicLink;
import floetteroed.utilities.networks.basic.BasicNetwork;
import floetteroed.utilities.networks.basic.BasicNode;

/**
 * Analyzes the (absolute/relative) frequency of individual paths in a given
 * sample.
 * 
 * @author Gunnar Flötteröd
 * 
 */
class FrequencyAnalyzer implements PathHandler {

	// -------------------- CONSTANTS --------------------

	private final BasicNetwork network;

	private final int sampleInterval;

	private final boolean printTotals;

	// -------------------- MEMBERS --------------------

	// TODO CHANGED
	private Map<List<BasicNode>, Integer> path2count = new HashMap<List<BasicNode>, Integer>();

	// TODO CHANGED
	private Map<List<BasicNode>, Double> path2LogWeight = new HashMap<List<BasicNode>, Double>();

	private double maxLogWeight;

	private PrintWriter writer = null;

	private String toFile = null;

	private int skipped = 0;

	// -------------------- CONSTRUCTION --------------------

	FrequencyAnalyzer(final BasicNetwork network, final int sampleInterval,
			final boolean printTotals) {
		if (network == null) {
			throw new IllegalArgumentException("network is null");
		}
		this.network = network;
		this.sampleInterval = sampleInterval;
		this.printTotals = printTotals;
	}

	// -------------------- IMPLEMENTATION --------------------

	void run(final String fromFile, final String toFile) throws IOException {
		if (fromFile == null) {
			throw new IllegalArgumentException("from file is null");
		}
		if (toFile == null) {
			throw new IllegalArgumentException("to file is null");
		}
		this.toFile = toFile;
		final PathXMLParser parser = new PathXMLParser(this.network);
		parser.parse(fromFile, this);

	}

	// -------------------- IMPLEMENTATION OF PathHandler --------------------

	@Override
	public void startPaths(final Attributes attrs) {
		try {
			this.writer = new PrintWriter(this.toFile);
		} catch (FileNotFoundException e) {
			throw new RuntimeException(e);
		}
	}

	@Override
	public void startOdPair(final BasicNode origin, final BasicNode destination) {
		this.path2count.clear();
		this.path2LogWeight.clear();
		this.maxLogWeight = Double.NEGATIVE_INFINITY;
		System.out.println("starting od pair " + origin.getId() + " -> "
				+ destination.getId());
		// TODO NEW
		this.skipped = 0;
	}

	@Override
	// TODO CHANGED
	public void startPath(final List<BasicNode> nodePath,
			final List<BasicLink> linkPath, final Attributes attrs) {
		if (this.skipped == this.sampleInterval - 1) {
			this.skipped = 0;
			final Integer oldCnt = this.path2count.get(nodePath);
			this.path2count.put(nodePath, (oldCnt == null ? 0 : oldCnt) + 1);
			final double logWeight = Double.parseDouble(attrs
					.getValue(MHPathWriterWrapper.LOGWEIGHT_ATTRIBUTE));
			this.path2LogWeight.put(nodePath, logWeight);
			this.maxLogWeight = Math.max(this.maxLogWeight, logWeight);
		} else {
			this.skipped++;
		}
	}

	@Override
	public void endPath() {
	}

	@Override
	public void endOdPair() {

		double totalCnt = 0;
		for (int cnt : this.path2count.values()) {
			totalCnt += cnt;
		}

		double totalWeight = 0;
		for (Double logWeight : this.path2LogWeight.values()) {
			totalWeight += Math.exp(logWeight - this.maxLogWeight);
		}

		// TODO CHANGED
		for (List<BasicNode> path : this.path2count.keySet()) {
			final double relFreq = Math.exp(this.path2LogWeight.get(path)
					- this.maxLogWeight)
					/ totalWeight;
			if (this.printTotals) {
				this.writer.print(relFreq * totalCnt);
				this.writer.print(" ");
				this.writer.println(this.path2count.get(path));
			} else {
				this.writer.print(relFreq);
				this.writer.print(" ");
				this.writer.println(this.path2count.get(path) / totalCnt);
			}
		}
		this.writer.println();
		this.writer.flush();
	}

	@Override
	public void endPaths() {
		this.writer.flush();
		this.writer.close();
	}
}
