package companyReviewService.dataModels;

/**
 * This class represents a client request to save a review of a company to the database
 * @author joshua walters
 */
public class SaveCompanyReviewRequest {
	public SaveCompanyReviewRequest(String jsonEncoding) throws Exception{
		parseJSONString(jsonEncoding);
	}
	
	/**
	 * Parses a JSON string representation of this object such that this object contains the information contained in the string 
	 * @param encoding - The JSON string representation
	 * @throws Exception - If the given string is not a valid format
	 */
	public void parseJSONString(String encoding) throws Exception{
		//Must separate the CustomerReview member's json encoding from the rest of this objects
		String reviewEncoding = "";
		int reviewLocation = encoding.indexOf("review") + "review\":".length();
		int reviewLength = 0;
		int depth = 1;
		do
		{
			if(encoding.charAt(reviewLocation + reviewLength) == '{'){
				depth++;
			}
			else if(encoding.charAt(reviewLocation + reviewLength) == '}'){
				depth--;
			}
			reviewLength++;
		} while(depth > 0);
		reviewEncoding = encoding.substring(reviewLocation, reviewLocation + reviewLength);
		this.review = new CustomerReview(reviewEncoding);
	}
	
	/**
	 * The customer review to save to the database
	 */
	private CustomerReview review;
	public CustomerReview getReview(){ return review; }
}
