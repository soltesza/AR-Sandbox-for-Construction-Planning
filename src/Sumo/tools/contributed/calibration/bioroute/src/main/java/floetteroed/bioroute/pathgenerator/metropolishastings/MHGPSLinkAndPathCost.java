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

import static floetteroed.bioroute.BiorouteRunner.PATHGENERATOR_ELEMENT;

import floetteroed.utilities.config.Config;
import floetteroed.utilities.math.MathHelpers;

/**
 * Computes link costs and path weights.
 * 
 * @author Gunnar Flötteröd
 * 
 */
class MHGPSLinkAndPathCost extends MHLinkAndPathCost {

	// -------------------- CONSTANTS --------------------

	// XML CONFIGURATION

	static final String GPSFILE_ELEMENT = "gpsfile";

	static final String LOGLIKELIHOODSCALE_ELEMENT = "loglikelihoodscale";

	// DEFAULT VALUES

	static final double DEFAULT_LOGLIKELIHOODSCALE = 1.0;

	// -------------------- MEMBERS --------------------

	// CONFIGURATION

	private String gpsFile = null;

	private Double logLikelihoodScale = null;

	private MHGPSLogLikelihood gpsLogLikelihood = null;

	// -------------------- CONSTRUCTION --------------------

	public MHGPSLinkAndPathCost() {
		// no-argument constructor for reflective instantiation
	}

	// -------------------- INITIALIZATION --------------------

	@Override
	void configure(final Config config) {

		super.configure(config);

		this.gpsFile = config.absolutePath(config.get(PATHGENERATOR_ELEMENT,
				GPSFILE_ELEMENT));
		this.gpsLogLikelihood = new MHGPSLogLikelihood();
		if (this.gpsFile != null) {
			this.gpsLogLikelihood.parse(this.gpsFile);
		}

		this.logLikelihoodScale = MathHelpers.parseDouble(config.get(
				PATHGENERATOR_ELEMENT, LOGLIKELIHOODSCALE_ELEMENT));
		if (this.logLikelihoodScale == null) {
			// TODO default values should be put into config object
			this.logLikelihoodScale = DEFAULT_LOGLIKELIHOODSCALE;
		}
	}

	// -------------------- IMPLEMENTATION OF MHWeight --------------------

	@Override
	double logWeightWithoutCorrection(final MHPath path) {
		return (super.logWeightWithoutCorrection(path) + this.logLikelihoodScale
				* this.gpsLogLikelihood.logLikelihood(path));
	}

}
