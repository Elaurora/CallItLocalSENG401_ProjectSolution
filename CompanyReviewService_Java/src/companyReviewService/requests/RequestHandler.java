package companyReviewService.requests;

import companyReviewService.database.*;
import companyReviewService.dataModels.*;

import java.sql.SQLException;

import javax.ws.rs.GET;
import javax.ws.rs.Path;
import javax.ws.rs.PathParam;
import javax.ws.rs.Produces;
import javax.ws.rs.core.Context;
import javax.ws.rs.core.Response;
import javax.ws.rs.core.UriInfo;

@Path("/")
public class RequestHandler {

	/**
	 * This is the function that is called when the server recieves a REST API request for a 
	 * list of customer reviews for a specific company
	 * @param info - The URL used to make the REST call
	 * @param JSONInfo - The JSON encoded request object
	 * @return A JSON encoded object containg a list of customer reviews
	 */
	@Path("GetCompanyReviews/{JSONInfo}")
	@GET
	@Produces({"application/json", "text/plain"})
	public Response HandleGetCompanyReview(@Context UriInfo info, @PathParam("JSONInfo") String JSONInfo){
		
		try{
			GetCompanyReviewsRequest request = new GetCompanyReviewsRequest(JSONInfo);
			GetCompanyReviewsResponse response = CompanyReviewDatabase.getInstance().getCompanyReviews(request);
			
			return Response.status(200)
					.entity(response.convertToJson())
					.type("application/json")
					.build();
		}
		catch(SQLException e){
			return Response.status(500)
					.entity("Error Code:" + String.valueOf(e.getErrorCode()) + " Error Message:" + e.getMessage())
					.type("text/plain")
					.build();
		}
		catch(ClassNotFoundException e){
			return Response.status(500)
					.entity("Error Message:" + e.getMessage())
					.type("text/plain")
					.build();
		}
		catch(Exception e){
			return Response.status(500)
					.entity("Error Message:" + e.getMessage())
					.type("text/plain")
					.build();
		}
	}
	
	/**
	 * This is the function that is called when the server recieves a REST API request to save 
	 * a customer review to the database
	 * @param info - The URL used to make the REST call
	 * @param JSONInfo - The JSON encoded request object
	 * @return A String response indicating success or failure, and an error message if relevant.
	 */
	@Path("SaveCompanyReview/{JSONInfo}")
	@GET
	@Produces({"text/plain"})
	public Response HandleSaveCompanyReviews(@Context UriInfo info, @PathParam("JSONInfo") String JSONInfo){
		String result;
		try{
			SaveCompanyReviewRequest request = new SaveCompanyReviewRequest(JSONInfo);
			result = CompanyReviewDatabase.getInstance().saveCompanyReview(request);
		}
		catch(Exception e){
			result = "Failure:" + e.getMessage();
		}
		if("Success".equals(result)){
			return Response.status(200)
					.entity(result)
					.type("text/plain")
					.build();
		}
		else{
			return Response.status(500)
					.entity(result)
					.type("text/plain)")
					.build();
		}
	}
}