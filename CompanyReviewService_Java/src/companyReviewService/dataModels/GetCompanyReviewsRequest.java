package companyReviewService.dataModels;

/**
 * This class represents a client request for the customer reviews of sa specific company
 * @author joshua walters
 */
public class GetCompanyReviewsRequest {
	public GetCompanyReviewsRequest(String jsonEncoding) throws Exception{
		parseJSONString(jsonEncoding);
	}
	
	/**
	 * Parses a JSON string representation of this object such that this object contains the information contained in the string 
	 * @param encoding - The JSON string representation
	 * @throws Exception - If the given string is not a valid format
	 */
	public void parseJSONString(String encoding) throws Exception{
		int companyNameLocation = encoding.indexOf("companyName");
		//First check and make sure the companyName information exists in the string as a json object
		if(companyNameLocation == -1){
			throw new Exception("Invalid jsonString given");
		}
		
		//Then adjust the index to point to the value we wish to read
		companyNameLocation += "companyName\":\"".length();
		
		//Finally, read the value
		this.companyName = encoding.substring(companyNameLocation, encoding.indexOf('"', companyNameLocation));
	}
	
	/**
	 * The name of the company to get the customer reviews for
	 */
	private String companyName;
	public String getCompanyName(){
		return companyName;
	}
}
