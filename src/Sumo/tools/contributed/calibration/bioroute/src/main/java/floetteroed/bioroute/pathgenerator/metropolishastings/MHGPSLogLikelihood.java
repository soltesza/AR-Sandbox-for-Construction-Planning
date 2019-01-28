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

import java.util.ArrayList;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;

import javax.xml.parsers.SAXParser;
import javax.xml.parsers.SAXParserFactory;

import org.xml.sax.Attributes;
import org.xml.sax.XMLReader;
import org.xml.sax.helpers.DefaultHandler;

import floetteroed.utilities.networks.basic.BasicNode;


/**
 * 
 * @author Gunnar Flötteröd
 * 
 */
class MHGPSLogLikelihood extends DefaultHandler {

	// -------------------- CONSTANTS --------------------

	final String GPSPOINT_ELEMENT = "gpspoint";

	final String DISTANCE_ELEMENT = "distance";

	final String LINK_ATTRIBUTE = "link";

	final String VALUE_ATTRIBUTE = "value";

	// -------------------- MEMBERS --------------------

	private final List<Map<String, Double>> link2distances = new ArrayList<Map<String, Double>>();

	// -------------------- CONSTRUCTION --------------------

	MHGPSLogLikelihood() {
	}

	// -------------------- PARSING --------------------

	void parse(final String gpsFile) {
		try {
			final SAXParserFactory factory = SAXParserFactory.newInstance();
			factory.setValidating(false);
			factory.setNamespaceAware(false);
			final SAXParser parser = factory.newSAXParser();
			final XMLReader reader = parser.getXMLReader();
			reader.setContentHandler(this);
			reader.setFeature("http://apache.org/xml/features/"
					+ "nonvalidating/load-external-dtd", false);
			reader.setFeature("http://xml.org/sax/features/" + "validation",
					false);
			reader.parse(gpsFile);
		} catch (Exception e) {
			throw new RuntimeException(e);
		}
	}

	// -------------------- CONTENT ACCESS --------------------

	double logLikelihood(final MHPath path) {
		double result = 0;
		for (Map<String, Double> link2dist : this.link2distances) {
			double minDist = Double.MAX_VALUE;
			for (BasicNode invertedNode : path.getNodes()) {
				final String linkId = invertedNode.getId();
				minDist = Math.min(minDist, link2dist.get(linkId));
			}
			result -= minDist * minDist;
		}
		return result;
	}

	// -------------------- OVERRIDING OF DefaultHandler --------------------

	private Map<String, Double> link2dist = null;

	@Override
	public void startDocument() {
		this.link2dist = null;
	}

	@Override
	public void startElement(final String uri, final String lName,
			final String qName, final Attributes attrs) {
		if (GPSPOINT_ELEMENT.equals(qName)) {
			this.link2dist = new LinkedHashMap<String, Double>();
		} else if (DISTANCE_ELEMENT.equals(qName)) {
			final String linkId = attrs.getValue(LINK_ATTRIBUTE);
			final Double dist = Double.parseDouble(attrs
					.getValue(VALUE_ATTRIBUTE));
			this.link2dist.put(linkId, dist);
		}
	}

	@Override
	public void endElement(final String uri, final String lName,
			final String qName) {
		if (GPSPOINT_ELEMENT.equals(qName)) {
			this.link2distances.add(this.link2dist);
			this.link2dist = null;
		}
	}
}
