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
import java.util.Arrays;
import java.util.HashSet;
import java.util.LinkedList;
import java.util.List;
import java.util.Set;

import org.xml.sax.Attributes;

import floetteroed.utilities.networks.basic.BasicLink;
import floetteroed.utilities.networks.basic.BasicNetwork;
import floetteroed.utilities.networks.basic.BasicNode;


/**
 * Analyzes the within-chain similarity of paths within a given sample.
 * 
 * @author Gunnar Flötteröd
 * 
 */
class SimilarityAnalyzer implements PathHandler {

	// -------------------- CONSTANTS --------------------

	// private final int sampleInterval;

	private final BasicNetwork network;

	// -------------------- MEMBERS --------------------

	private String toFile = null;

	private PrintWriter writer = null;

	// TODO CHANGED
	private final LinkedList<List<BasicNode>> routeBuffer = new LinkedList<List<BasicNode>>();

	private final double[] binCnt;

	private long totalCnt;

	// -------------------- CONSTRUCTION --------------------

	SimilarityAnalyzer(final int maxDist, // final int sampleInterval,
			final BasicNetwork network) {
		// if (sampleInterval < 1) {
		// throw new IllegalArgumentException("sample interval < 1");
		// }
		if (maxDist < 0) {
			throw new IllegalArgumentException("max. dist. < 0");
		}
		if (network == null) {
			throw new IllegalArgumentException("network is null");
		}
		// this.sampleInterval = sampleInterval;
		// this.binCnt = new double[maxDist / sampleInterval + 1];
		this.binCnt = new double[maxDist + 1];
		this.network = network;
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
		this.totalCnt = 0;
		Arrays.fill(this.binCnt, 0.0);
		this.routeBuffer.clear();
		System.out.println("starting od pair " + origin.getId() + " -> "
				+ destination.getId());
	}

	@Override
	// TODO CHANGED
	public void startPath(final List<BasicNode> nodePath,
			final List<BasicLink> linkPath, final Attributes attrs) {
		/*
		 * (1) update the buffer
		 */
		this.routeBuffer.addFirst(nodePath);
		while (this.routeBuffer.size() > this.binCnt.length) {
			this.routeBuffer.removeLast();
		}
		/*
		 * (2) update the statistics
		 */
		if (this.routeBuffer.size() == this.binCnt.length) {
			this.binCnt[0] += 1.0;
			final Set<BasicNode> nodes1 = new HashSet<BasicNode>(
					this.routeBuffer.getFirst());
			for (int bin = 1; bin < this.binCnt.length; bin++) {
				final Set<BasicNode> nodes2 = new HashSet<BasicNode>(
						this.routeBuffer.get(bin));
				final double denom = 0.5 * (nodes1.size() + nodes2.size());
				nodes2.retainAll(nodes1);
				this.binCnt[bin] += nodes2.size() / denom;
			}
			this.totalCnt++;
		}
	}

	@Override
	public void endPath() {
	}

	@Override
	public void endOdPair() {
		for (int i = 0; i < this.binCnt.length; i++) {
			// this.writer.print(i * this.sampleInterval);
			this.writer.print(i);
			this.writer.print(" ");
			this.writer.println(this.binCnt[i] / this.totalCnt);
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
