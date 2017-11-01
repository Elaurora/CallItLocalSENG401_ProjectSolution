package companyReviewService.dataModels;

import java.util.ArrayList;

/**
 * This class represents the response that will be sent back to the client that make the request for a companys reviews
 * @author joshua walters
 */
public class GetCompanyReviewsResponse {
	public GetCompanyReviewsResponse(ArrayList<CustomerReview> reviews, String response, boolean result){
		this.reviews = reviews;
		this.response = response;
		this.result = result;
	}
	
	/**
	 * Converts the current state of this object into a JSON string representation
	 * @return A JSON string representation of this objects current state
	 */
	public String convertToJson(){
		String jsonEncoding = "{\"reviews\":[";
		if(reviews.size() != 0){
			int i = 0;
			while(true){
				jsonEncoding += reviews.get(i++).convertToJson();
				if(i == reviews.size()){
					break;
				}
				jsonEncoding += ',';
			}
		}
		jsonEncoding += "]";
		
		jsonEncoding += ",\"result\":" + String.valueOf(result);
		jsonEncoding += ",\"response\":\"" + response + "\"";
		
		jsonEncoding += "}";
		
		return jsonEncoding;
	}
	
	/**
	 * Indicates whether or not the server was able to successfully complete the request
	 */
	private boolean result;
	public boolean getResult(){ return result; }
	
	/**
	 * Information about the outcome of the request
	 */
	private String response = "";
	public String getResponse(){ return response; }
	
	/**
	 * A list of customer reviews
	 */
	private ArrayList<CustomerReview> reviews;
	public ArrayList<CustomerReview> getReviews(){ return reviews; }
}
