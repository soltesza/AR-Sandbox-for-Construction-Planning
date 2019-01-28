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

import floetteroed.bioroute.NetworkPreprocessor;
import floetteroed.utilities.config.Config;
import floetteroed.utilities.math.MathHelpers;
import floetteroed.utilities.networks.basic.BasicLink;
import floetteroed.utilities.networks.basic.BasicNetwork;
import floetteroed.utilities.networks.basic.BasicNode;

/**
 * Preprocessor for SUMO networks. Infers link lengths from node coordinates.
 * 
 * @author Gunnar Flötteröd
 * 
 */
public class SUMOPreprocessor implements NetworkPreprocessor {

	// -------------------- CONSTANTS --------------------

	/**
	 * Required node attribute for the preprocessing.
	 */
	public static final String NODE_X_ATTRIBUTE = "x";

	/**
	 * Required node attribute for the preprocessing.
	 */
	public static final String NODE_Y_ATTRIBUTE = "y";

	/**
	 * Available link attribute after the preprocessing.
	 */
	public static final String NAIVELENGTH_ATTRIBUTE = "naivelength";

	// -------------------- CONSTRUCTION --------------------

	public SUMOPreprocessor() {
	}

	// --------------- IMPLEMENTATION OF NetworkPreprocessor ---------------

	@Override
	public void configure(final Config config) {
	}

	@Override
	public void preprocess(final BasicNetwork network) {
		for (BasicLink link : network.getLinks()) {
			final BasicNode from = link.getFromNode();
			final BasicNode to = link.getToNode();
			final double naiveLength = MathHelpers.length(Double
					.parseDouble(from.getAttr(NODE_X_ATTRIBUTE)), Double
					.parseDouble(from.getAttr(NODE_Y_ATTRIBUTE)), Double
					.parseDouble(to.getAttr(NODE_X_ATTRIBUTE)), Double
					.parseDouble(to.getAttr(NODE_Y_ATTRIBUTE)));
			link.setAttr(NAIVELENGTH_ATTRIBUTE, Double.toString(naiveLength));
		}
	}
}
