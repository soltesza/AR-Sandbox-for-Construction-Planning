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
package floetteroed.bioroute.pathgenerator.metropolishastings;

import java.util.HashMap;
import java.util.Map;

import floetteroed.bioroute.PathWriter;
import floetteroed.utilities.math.metropolishastings.MHStateProcessor;

/**
 * Wraps a <code>PathWriter</code> into a <code>MHStateProcessor</code>.
 * 
 * @author Gunnar Flötteröd
 * 
 */
public class MHPathWriterWrapper implements MHStateProcessor<MHPath> {

	// -------------------- CONSTANTS --------------------

	public static final String LOGWEIGHT_ATTRIBUTE = "logweight";

	// -------------------- MEMBERS --------------------

	// CONFIGURATION

	private final PathWriter pathWriter;

	private final int sampleInterval;

	private final MHLinkAndPathCost pathCost;

	// RUNTIME

	private int pathCnt;

	// -------------------- CONSTRUCTION --------------------

	MHPathWriterWrapper(final PathWriter pathWriter, final int sampleInterval,
			final MHLinkAndPathCost pathCost) {
		if (pathWriter == null) {
			throw new IllegalArgumentException("path writer is null");
		}
		if (sampleInterval <= 0) {
			throw new IllegalArgumentException(
					"sample interval is not strictly positive");
		}
		if (pathCost == null) {
			throw new IllegalArgumentException("path cost is null");
		}
		this.pathWriter = pathWriter;
		this.sampleInterval = sampleInterval;
		this.pathCost = pathCost;
	}

	// --------------- IMPLEMENTATION OF MHStateProcessor ---------------

	@Override
	public void start() {
		this.pathCnt = 0;
	}

	@Override
	public void processState(final MHPath path) {
		/*
		 * (1) check if this path should be written
		 */
		this.pathCnt++;
		if (this.pathCnt % this.sampleInterval != 0) {
			return;
		}
		/*
		 * (2) write out the link IDs that correspond to the inverted nodes
		 */
		// final List<BasicNode> invertedNodes = path.getNodes();
		// final List<String> ids = new ArrayList<String>(invertedNodes.size());
		// for (BasicNode invertedNode : invertedNodes) {
		// ids.add(invertedNode.getId());
		// }
		final Map<String, String> attrs = new HashMap<String, String>();
		attrs.put(LOGWEIGHT_ATTRIBUTE,
				Double.toString(this.pathCost.logWeightWithoutCorrection(path)));
		// TODO NEW
		// this.pathWriter.writePath(ids, attrs);
		this.pathWriter.writePath(path.getLinks(), attrs);
	}

	@Override
	public void end() {
	}
}
