﻿using ClientApplicationMVC.Models;

using Messages.DataTypes.Database.CompanyDirectory;
using Messages.DataTypes.Database.CompanyReview;
using Messages.ServiceBusRequest;
using Messages.ServiceBusRequest.CompanyDirectory.Responses;
using Messages.ServiceBusRequest.CompanyDirectory.Requests;
using Messages.ServiceBusRequest.CompanyReview.Requests;
using Messages.ServiceBusRequest.CompanyReview.Responses;

using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace ClientApplicationMVC.Controllers
{
    /// <summary>
    /// This class contains the functions responsible for handling requests routed to *Hostname*/CompanyListings/*
    /// </summary>
    public class CompanyListingsController : Controller
    {
        /// <summary>
        /// This function is called when the client navigates to *hostname*/CompanyListings
        /// </summary>
        /// <returns>A view to be sent to the client</returns>
        public ActionResult Index()
        {
            if (Globals.isLoggedIn())
            {
                ViewBag.Companylist = null;
                return View("Index");
            }
            return RedirectToAction("Index", "Authentication");
        }

        /// <summary>
        /// This function is called when the client navigates to *hostname*/CompanyListings/Search
        /// </summary>
        /// <returns>A view to be sent to the client</returns>
        public ActionResult Search(string textCompanyName)
        {

            if (Globals.isLoggedIn() == false)
            {
                return RedirectToAction("Index", "Authentication");
            }

            ServiceBusConnection connection = ConnectionManager.getConnectionObject(Globals.getUser());
            if(connection == null)
            {
                return RedirectToAction("Index", "Authentication");
            }

            CompanySearchRequest request = new CompanySearchRequest(textCompanyName);

            CompanySearchResponse response = connection.searchCompanyByName(request);
            if (response.result == false)
            {
                return RedirectToAction("Index", "Authentication");
            }

            ViewBag.Companylist = response.list;

            return View("Index");
        }

        /// <summary>
        /// This function is called when the client navigates to *hostname*/CompanyListings/DisplayCompany/*info*
        /// </summary>
        /// <param name="info">The name of the company whos info is to be displayed</param>
        /// <returns>A view to be sent to the client</returns>
        public ActionResult DisplayCompany(string info)
        {
            if (Globals.isLoggedIn() == false)
            {
                return RedirectToAction("Index", "Authentication");
            }
            if ("".Equals(info))
            {
                return View("Index");
            }

            ServiceBusConnection connection = ConnectionManager.getConnectionObject(Globals.getUser());
            if (connection == null)
            {
                return RedirectToAction("Index", "Authentication");
            }

            ViewBag.CompanyName = info;

            GetCompanyInfoRequest infoRequest = new GetCompanyInfoRequest(new CompanyInstance(info));
            GetCompanyInfoResponse infoResponse = connection.getCompanyInfo(infoRequest);
            ViewBag.CompanyInfo = infoResponse.companyInfo;

            GetCompanyReviewsRequest reviewRequest = new GetCompanyReviewsRequest(info);
            GetCompanyReviewsResponse reviewResponse = connection.getCompanyReviews(reviewRequest);
            ViewBag.CompanyReviews = reviewResponse.getReviews();

            return View("DisplayCompany");
        }

        [HttpPost]
        public ActionResult WriteReview(string company = "", string userReview = "", int timestamp = -1, int stars = -1)
        {
            if (Globals.isLoggedIn() == false 
                || company == "" || userReview == "" 
                || timestamp == -1 || stars == -1)
            {
                return RedirectToAction("Index", "Authentication");
            }

            ServiceBusConnection connection = ConnectionManager.getConnectionObject(Globals.getUser());
            if (connection == null)
            {
                return RedirectToAction("Index", "Authentication");
            }

            SaveCompanyReviewRequest request = new SaveCompanyReviewRequest(new CustomerReview(company, Globals.getUser(), userReview, stars, timestamp));

            ServiceBusResponse response = connection.saveCompanyReview(request);

            return null;
        }
    }
}