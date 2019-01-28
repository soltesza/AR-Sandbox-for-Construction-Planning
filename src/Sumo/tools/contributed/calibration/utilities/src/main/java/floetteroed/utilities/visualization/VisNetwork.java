/*
 * Copyright 2015, 2016 Gunnar Flötteröd
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *
 * contact: gunnar.floetteroed@abe.kth.se
 *
 */ 
package floetteroed.utilities.visualization;

import floetteroed.utilities.networks.construction.AbstractNetwork;

/**
 * 
 * @author Gunnar Flötteröd
 *
 */
public class VisNetwork extends AbstractNetwork<VisNode, VisLink> {

	// -------------------- MEMBERS --------------------

	private double minEasting;

	private double maxEasting;

	private double minNorthing;

	private double maxNorthing;

	// private final Map<BasicNode, VisNodeData> nodeData = new
	// HashMap<BasicNode, VisNodeData>();

	// private final Map<BasicLink, VisLinkData> linkData = new
	// HashMap<BasicLink, VisLinkData>();

	// -------------------- CONSTRUCTION --------------------

	public VisNetwork(String id, String type) {
		super(id, type);
	}

	// -------------------- WRITE ACCESS --------------------

	public void setMinEasting(final double minEasting) {
		this.minEasting = minEasting;
	}

	public void setMaxEasting(final double maxEasting) {
		this.maxEasting = maxEasting;
	}

	public void setMinNorthing(final double minNorthing) {
		this.minNorthing = minNorthing;
	}

	public void setMaxNorthing(final double maxNorthing) {
		this.maxNorthing = maxNorthing;
	}

	// -------------------- READ ACCESS --------------------

	public double getMinEasting() {
		return this.minEasting;
	}

	public double getMaxEasting() {
		return this.maxEasting;
	}

	public double getMinNorthing() {
		return this.minNorthing;
	}

	public double getMaxNorthing() {
		return this.maxNorthing;
	}

	// VisNodeData getVisNodeData(final BasicNode node) {
	// VisNodeData result = this.nodeData.get(node);
	// if (result == null) {
	// result = new VisNodeData();
	// this.nodeData.put(node, result);
	// }
	// return result;
	// }

	// VisLinkData getVisLinkData(final BasicLink link) {
	// VisLinkData result = this.linkData.get(link);
	// if (result == null) {
	// result = new VisLinkData();
	// this.linkData.put(link, result);
	// }
	// return result;
	// }
}
