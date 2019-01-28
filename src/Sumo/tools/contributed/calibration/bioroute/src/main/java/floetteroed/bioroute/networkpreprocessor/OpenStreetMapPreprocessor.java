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
package floetteroed.bioroute.networkpreprocessor;

import java.awt.geom.Point2D;
import java.util.Map;

import floetteroed.bioroute.NetworkPreprocessor;
import floetteroed.utilities.config.Config;
import floetteroed.utilities.math.MathHelpers;
import floetteroed.utilities.networks.basic.BasicLink;
import floetteroed.utilities.networks.basic.BasicNetwork;
import floetteroed.utilities.networks.basic.BasicNode;
import floetteroed.utilities.visualization.OpenStreetMap2VisNetwork;

/**
 * Preprocessor for OpenStreetMap networks. Infers Euklidean node coordinates
 * and link lengths (naive, projected) from node longitudes/latitudes.
 * 
 * @author Gunnar Flötteröd
 * 
 */
public class OpenStreetMapPreprocessor implements NetworkPreprocessor {

	// -------------------- CONSTANTS --------------------

	/**
	 * Available link attribute after the prep-rocessing.
	 */
	public static final String NAIVELENGTH_ATTRIBUTE = "naivelength";

	/**
	 * Available link attribute after the pre-processing.
	 */
	public static final String PROJECTEDLENGTH_ATTRIBUTE = "projectedlength";

	// -------------------- CONSTRUCTION --------------------

	public OpenStreetMapPreprocessor() {
	}

	// --------------- IMPLEMENTATION OF NetworkPreprocessor ---------------

	@Override
	public void configure(final Config config) {
	}

	@Override
	public void preprocess(final BasicNetwork network) {
		/*
		 * (1) extract node coordinates
		 */
		// final Map<BasicNode, Point2D.Double> node2LonLat =
		// OpenStreetMapNetVisPreprocessor
		// .node2LonLat(network);
		// final Map<BasicNode, Point2D.Double> node2xy =
		// OpenStreetMapNetVisPreprocessor
		// .node2xy(node2LonLat);
		final Map<BasicNode, Point2D.Double> node2LonLat = OpenStreetMap2VisNetwork
				.node2LonLat(network);
		final Map<BasicNode, Point2D.Double> node2xy = OpenStreetMap2VisNetwork
				.node2xy(node2LonLat);
		/*
		 * (2) add link attributes
		 */
		for (BasicLink link : network.getLinks()) {
			/*
			 * (2.1) naive length
			 */
			final Point2D.Double fromLonLat = node2LonLat.get(link
					.getFromNode());
			final Point2D.Double toLonLat = node2LonLat.get(link.getToNode());
			final double naiveLength = MathHelpers.length(fromLonLat.x,
					fromLonLat.y, toLonLat.x, toLonLat.y);
			link.setAttr(NAIVELENGTH_ATTRIBUTE, Double.toString(naiveLength));
			/*
			 * (2.2) projected length
			 */
			final Point2D.Double fromXY = node2xy.get(link.getFromNode());
			final Point2D.Double toXY = node2xy.get(link.getToNode());
			final double projectedLength = MathHelpers.length(fromXY.x,
					fromXY.y, toXY.x, toXY.y);
			link.setAttr(PROJECTEDLENGTH_ATTRIBUTE,
					Double.toString(projectedLength));
		}
	}
}
