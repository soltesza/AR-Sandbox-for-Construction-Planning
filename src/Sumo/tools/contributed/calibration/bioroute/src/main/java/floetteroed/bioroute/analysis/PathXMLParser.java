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

import static floetteroed.bioroute.pathwriter.PathXMLWriter.FROM_ATTRIBUTE;
import static floetteroed.bioroute.pathwriter.PathXMLWriter.LINKS_ATTRIBUTE;
import static floetteroed.bioroute.pathwriter.PathXMLWriter.NODES_ATTRIBUTE;
import static floetteroed.bioroute.pathwriter.PathXMLWriter.ODPAIR_ELEMENT;
import static floetteroed.bioroute.pathwriter.PathXMLWriter.PATHS_ELEMENT;
import static floetteroed.bioroute.pathwriter.PathXMLWriter.PATH_ELEMENT;
import static floetteroed.bioroute.pathwriter.PathXMLWriter.TO_ATTRIBUTE;

import java.util.ArrayList;
import java.util.List;

import javax.xml.parsers.SAXParser;
import javax.xml.parsers.SAXParserFactory;

import org.xml.sax.Attributes;
import org.xml.sax.XMLReader;
import org.xml.sax.helpers.DefaultHandler;

import floetteroed.utilities.networks.basic.BasicLink;
import floetteroed.utilities.networks.basic.BasicNetwork;
import floetteroed.utilities.networks.basic.BasicNode;


/**
 * Parser for path XML files created by
 * <code>bioroute.pathwriter.PathXMLWriter</code>. Calls an instance of
 * <code>PathHandler</code>.
 * 
 * @author Gunnar Flötteröd
 * 
 */
public class PathXMLParser extends DefaultHandler {

	// -------------------- MEMBERS --------------------

	private final BasicNetwork network;

	private PathHandler handler = null;

	// -------------------- CONSTRUCTION --------------------

	public PathXMLParser(final BasicNetwork network) {
		if (network == null) {
			throw new IllegalArgumentException("network is null");
		}
		this.network = network;
	}

	// -------------------- IMPLEMENTATION --------------------

	public void parse(final String file, final PathHandler handler) {
		if (file == null) {
			throw new IllegalArgumentException("file is null");
		}
		if (handler == null) {
			throw new IllegalArgumentException("handler is null");
		}
		this.handler = handler;
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
			reader.parse(file);
		} catch (Exception e) {
			throw new RuntimeException(e);
		}
	}

	// -------------------- Overriding of DefaultHandler --------------------

	@Override
	public void startElement(final String uri, final String lName,
			final String qName, final Attributes attrs) {
		if (PATHS_ELEMENT.equals(qName)) {
			this.handler.startPaths(attrs);
		} else if (ODPAIR_ELEMENT.equals(qName)) {
			// final BasicLink origin = this.network.getLink(attrs
			// .getValue(FROM_ATTRIBUTE));
			// final BasicLink destination = this.network.getLink(attrs
			// .getValue(TO_ATTRIBUTE));
			final BasicNode origin = this.network.getNode(attrs
					.getValue(FROM_ATTRIBUTE));
			final BasicNode destination = this.network.getNode(attrs
					.getValue(TO_ATTRIBUTE));
			this.handler.startOdPair(origin, destination);
		} else if (PATH_ELEMENT.equals(qName)) {
			// >>>>> TODO CHANGED >>>>>
			final List<BasicNode> nodePath = new ArrayList<BasicNode>();
			for (String nodeId : attrs.getValue(NODES_ATTRIBUTE).split("\\s")) {
				nodePath.add(this.network.getNode(nodeId.trim()));
			}
			final List<BasicLink> linkPath = new ArrayList<BasicLink>();
			for (String linkId : attrs.getValue(LINKS_ATTRIBUTE).split("\\s")) {
				linkPath.add(this.network.getLink(linkId.trim()));
			}
			this.handler.startPath(nodePath, linkPath, attrs);
			// final List<Link> path = new ArrayList<Link>();
			// for (String linkId :
			// attrs.getValue(LINKS_ATTRIBUTE).split("\\s")) {
			// path.add(this.network.getLink(linkId.trim()));
			// }
			// this.handler.startPath(path, attrs);
			// <<<<< TODO CHANGED <<<<<
		}
	}

	@Override
	public void endElement(final String uri, final String lName,
			final String qName) {
		if (PATHS_ELEMENT.equals(qName)) {
			this.handler.endPaths();
		} else if (ODPAIR_ELEMENT.equals(qName)) {
			this.handler.endOdPair();
		} else if (PATH_ELEMENT.equals(qName)) {
			this.handler.endPath();
		}
	}
}
