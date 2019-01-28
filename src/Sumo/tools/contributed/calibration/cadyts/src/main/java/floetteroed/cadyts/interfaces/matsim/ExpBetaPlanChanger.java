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
package floetteroed.cadyts.interfaces.matsim;

import static java.lang.Math.exp;
import static java.lang.Math.min;

import java.util.ArrayList;
import java.util.List;
import java.util.Random;

import floetteroed.utilities.math.MathHelpers;
import floetteroed.utilities.math.Matrix;
import floetteroed.utilities.math.Vector;



/**
 * 
 * @author Gunnar Flötteröd
 * 
 */
public class ExpBetaPlanChanger {

	// -------------------- EXOGENEOUS PARAMETERS --------------------

	private double utilityScale;

	private final Vector coeff;

	private final Vector asc;

	private final Matrix attr;

	// -------------------- ENDOGENEOUS PARAMETERS --------------------

	private int currentChoice;

	private final Vector utilities;

	private final Vector choiceProbs;

	private final Matrix dProbs_dCoeffs;

	private final Matrix dProbs_dASCs;

	private boolean consistent;

	private boolean inSmoothConditions;

	// -------------------- CONSTRUCTION --------------------

	public ExpBetaPlanChanger(final int choiceSetSize, final int attributeCount) {
		this.utilityScale = 1.0;
		this.coeff = new Vector(attributeCount);
		this.asc = new Vector(choiceSetSize);
		this.attr = new Matrix(choiceSetSize, attributeCount);
		this.utilities = new Vector(choiceSetSize);
		this.choiceProbs = new Vector(choiceSetSize);
		this.dProbs_dCoeffs = new Matrix(choiceSetSize, attributeCount);
		this.dProbs_dASCs = new Matrix(choiceSetSize, choiceSetSize);
		this.consistent = false;
	}

	// -------------------- SETTERS --------------------

	public void setUtilityScale(final double value) {
		this.consistent = false;
		this.utilityScale = value;
	}

	public void setCoefficient(final int attrIndex, final double value) {
		this.consistent = false;
		this.coeff.set(attrIndex, value);
	}

	public void setASC(final int choiceIndex, final double value) {
		this.consistent = false;
		this.asc.set(choiceIndex, value);
	}

	public void setAttribute(final int choiceIndex, final int attrIndex,
			final double value) {
		this.consistent = false;
		this.attr.getRow(choiceIndex).set(attrIndex, value);
	}

	public void setCurrentChoice(final int currentChoice) {
		this.consistent = false;
		this.currentChoice = currentChoice;
	}

	// -------------------- UPDATE --------------------

	public void enforcedUpdate() {

		/*
		 * (1) update utilities
		 */
		for (int i = 0; i < this.getChoiceSetSize(); i++) {
			final double v = this.coeff.innerProd(this.attr.getRow(i))
					+ this.asc.get(i);
			this.utilities.set(i, v);
		}

		/*
		 * (2) update choice probabilities
		 */
		final boolean[] inSmoothConditions = new boolean[this
				.getChoiceSetSize()]; // currentChoice index is undefined!
		this.inSmoothConditions = true;
		double pSum = 0;
		double currentUtility = this.utilities.get(this.currentChoice);
		for (int i = 0; i < this.getChoiceSetSize(); i++) {
			if (i != this.currentChoice) {
				double p = min(1.0, 0.01 * exp(0.5 * this.utilityScale
						* (this.utilities.get(i) - currentUtility)));
				this.inSmoothConditions &= (p < 1.0);
				inSmoothConditions[i] = (p < 1.0);
				p /= this.getChoiceSetSize();
				this.choiceProbs.set(i, p);
				pSum += p;
			}
		}
		this.choiceProbs.set(this.currentChoice, 1.0 - pSum);

		/*
		 * (3) update derivatives of choice probabilities w.r.t. coefficients
		 */
		this.dProbs_dCoeffs.clear();
		final Vector attrCurrent = this.attr.getRow(this.currentChoice);
		final Vector dProbsCurrent_dCoeffs = this.dProbs_dCoeffs
				.getRow(this.currentChoice);
		for (int i = 0; i < this.getChoiceSetSize(); i++) {
			if (i != this.currentChoice && inSmoothConditions[i]) {
				final Vector dProbi_dCoeff = this.dProbs_dCoeffs.getRow(i);
				final double probi = this.choiceProbs.get(i);
				final Vector attri = this.attr.getRow(i);
				for (int j = 0; j < this.getAttrCount(); j++) {
					final double dProbi_dCoeffj = probi * 0.5
							* this.utilityScale
							* (attri.get(j) - attrCurrent.get(j));
					dProbi_dCoeff.set(j, dProbi_dCoeffj);
					dProbsCurrent_dCoeffs.add(j, -dProbi_dCoeffj);
				}
			}
		}

		/*
		 * (3) update derivatives of choice probabilities w.r.t. ASCs
		 */
		this.dProbs_dASCs.clear(); // TODO eventually when necessary

		this.consistent = true;
	}

	public void conditionalUpdate() {
		if (!this.consistent) {
			this.enforcedUpdate();
		}
	}

	// -------------------- GETTERS AND THE LIKE --------------------

	public int getChoiceSetSize() {
		return this.choiceProbs.size();
	}

	public int getAttrCount() {
		return this.coeff.size();
	}

	public Vector getCoeff() {
		return this.coeff.newImmutableView();
	}

	public Vector getASC() {
		return this.asc.newImmutableView();
	}

	public Vector getProbs() {
		this.conditionalUpdate();
		return this.choiceProbs.newImmutableView();
	}

	public Vector getUtils() {
		this.conditionalUpdate();
		return this.utilities.newImmutableView();
	}

	public Matrix get_dProbs_dCoeffs() {
		this.conditionalUpdate();
		return this.dProbs_dCoeffs.newImmutableView();
	}

	public Matrix get_dProbs_dASCs() {
		this.conditionalUpdate();
		return this.dProbs_dASCs.newImmutableView();
	}

	public boolean getInSmoothConditions() {
		this.conditionalUpdate();
		return this.inSmoothConditions;
	}

	// -------------------- MISCELLANEOUS --------------------

	public int draw(final Random rnd) {
		return MathHelpers.draw(this.getProbs(), rnd);
	}

	// --------------- PARAMETER VECTOR EXTERNALIZATION ---------------

	public int getParameterSize(final boolean withASC) {
		return this.getAttrCount() + (withASC ? this.getChoiceSetSize() : 0);
	}

	public int getParameterSize(final List<Integer> attributeIndices,
			final boolean withASC) {
		return attributeIndices.size()
				+ (withASC ? this.getChoiceSetSize() : 0);
	}

	public double getParameter(final int j) {
		if (j < this.getAttrCount()) {
			return this.getCoeff().get(j);
		} else {
			return this.getASC().get(j - this.getAttrCount());
		}
	}

	public Vector getParameters(final boolean withASC) {
		final Vector result = new Vector(this.getParameterSize(withASC));
		for (int j = 0; j < result.size(); j++) {
			result.set(j, this.getParameter(j));
		}
		return result;
	}

	public void setParameter(final int j, final double value) {
		this.consistent = false;
		if (j < this.getAttrCount()) {
			this.setCoefficient(j, value);
		} else {
			this.setASC(j - this.getAttrCount(), value);
		}
	}

	public void setParameters(final Vector parameters,
			final List<Integer> attributeIndices) {
		if (parameters.size() != attributeIndices.size()) {
			throw new IllegalArgumentException(
					"arguments must have the same dimension");
		}
		this.consistent = false;
		for (int j = 0; j < parameters.size(); j++) {
			this.setParameter(attributeIndices.get(j), parameters.get(j));
		}
		// this.consistent = false;
		// for (int j = 0; j < parameters.size(); j++) {
		// this.setParameter(j, parameters.get(j));
		// }
	}

	public Matrix get_dProb_dParameters(final List<Integer> attributeIndices,
			final boolean withASC) {
		this.conditionalUpdate();
		final Matrix result = new Matrix(this.getChoiceSetSize(), this
				.getParameterSize(attributeIndices, withASC));
		for (int i = 0; i < this.getChoiceSetSize(); i++) {
			final Vector resulti = result.getRow(i);
			final Vector dProbi_dCoeff = this.get_dProbs_dCoeffs().getRow(i);
			final Vector dProbi_dASC = this.get_dProbs_dASCs().getRow(i);
			int l = 0;
			for (int j : attributeIndices) {
				// for (int j = 0; j < this.getAttrCount(); j++) {
				resulti.set(l++, dProbi_dCoeff.get(j));
			}
			if (withASC) {
				for (int i2 = 0; i2 < this.getChoiceSetSize(); i2++) {
					resulti.set(l++, dProbi_dASC.get(i2));
				}
			}
		}
		return result;
		// this.conditionalUpdate();
		// final Matrix result = new Matrix(this.getChoiceSetSize(), this
		// .getParameterSize(withASC));
		// for (int i = 0; i < this.getChoiceSetSize(); i++) {
		// final Vector resulti = result.getRow(i);
		// final Vector dProbi_dCoeff = this.get_dProbs_dCoeffs().getRow(i);
		// final Vector dProbi_dASC = this.get_dProbs_dASCs().getRow(i);
		// int l = 0;
		// for (int j = 0; j < this.getAttrCount(); j++) {
		// resulti.set(l++, dProbi_dCoeff.get(j));
		// }
		// if (withASC) {
		// for (int i2 = 0; i2 < this.getChoiceSetSize(); i2++) {
		// resulti.set(l++, dProbi_dASC.get(i2));
		// }
		// }
		// }
		// return result;
	}

	/**
	 * Provides a numerical approximation of the Hessian of each choice
	 * probability with respect to the model parameters.
	 * 
	 * @param delta
	 *            the step size of the finite differences
	 * 
	 * @param withASC
	 *            if the alternative specific constants are to be accounted for
	 * 
	 * @returns a numerical approximation of the Hessian of each choice
	 *          probability with respect to the model parameters
	 */
	public List<Matrix> get_d2P_dbdb(final double delta,
			final List<Integer> attributeIndices, final boolean withASC) {

		final List<Matrix> result = new ArrayList<Matrix>(this
				.getChoiceSetSize());
		final int paramSize = this.getParameterSize(attributeIndices, withASC);
		for (int i = 0; i < this.getChoiceSetSize(); i++) {
			result.add(new Matrix(paramSize, paramSize));
		}

		final Matrix dP_db0 = this.get_dProb_dParameters(attributeIndices,
				withASC).copy();
		int resultIndex = 0; // TODO not nice...
		for (int r : attributeIndices) { // TODO ASCs...
			final double br0 = this.getParameter(r);
			this.setParameter(r, br0 + delta);
			for (int i = 0; i < this.getChoiceSetSize(); i++) {
				final Vector d2Pi_dbrdb = result.get(i).getRow(resultIndex);
				d2Pi_dbrdb.add(this.get_dProb_dParameters(attributeIndices,
						withASC).getRow(i), +1.0);
				d2Pi_dbrdb.add(dP_db0.getRow(i), -1.0);
				d2Pi_dbrdb.mult(1.0 / delta);
			}
			this.setParameter(r, br0);
			resultIndex++;
		}

		return result;

		// final List<Matrix> result = new ArrayList<Matrix>(this
		// .getChoiceSetSize());
		// for (int i = 0; i < this.getChoiceSetSize(); i++) {
		// result.add(new Matrix(this.getParameterSize(withASC), this
		// .getParameterSize(withASC)));
		// }
		//
		// final Matrix dP_db0 = this.get_dProb_dParameters(withASC).copy();
		// for (int r = 0; r < this.getParameterSize(withASC); r++) {
		// final double br0 = this.getParameter(r);
		// this.setParameter(r, br0 + delta);
		// for (int i = 0; i < this.getChoiceSetSize(); i++) {
		// final Vector d2Pi_dbrdb = result.get(i).getRow(r);
		// d2Pi_dbrdb.add(this.get_dProb_dParameters(withASC).getRow(i),
		// +1.0);
		// d2Pi_dbrdb.add(dP_db0.getRow(i), -1.0);
		// d2Pi_dbrdb.mult(1.0 / delta);
		// }
		// this.setParameter(r, br0);
		// }
		//
		// return result;
	}
}
