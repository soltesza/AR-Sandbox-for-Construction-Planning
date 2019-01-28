package floetteroed.bioroute.examples.michelemma;

import floetteroed.bioroute.BiorouteRunner;
import floetteroed.bioroute.analysis.AnalysisRunner;
import floetteroed.utilities.visualization.NetvisFromFileRunner;

/**
 * 
 * @author Gunnar Flötteröd
 *
 */
public class RunMichelEmma {

	public static void main(String[] args) {
		
		final String testdataPath = "./testdata/MichelEmmaNetwork/";
		final String biorouteConfig = testdataPath + "config.xml";
		final String netvisConfig = testdataPath + "vis-config.xml";

		BiorouteRunner.main(new String[] { biorouteConfig });

		AnalysisRunner.main(new String[] { "VISUAL", "-CONFIGFILE",
				biorouteConfig, "-VISCONFIGFILE", netvisConfig, "-VISDATAFILE",
				testdataPath + "vis-data.xml" });

		// set link with to 250 to see something meaningful
		NetvisFromFileRunner.main(new String[] { netvisConfig });		

		
	}
	
}
