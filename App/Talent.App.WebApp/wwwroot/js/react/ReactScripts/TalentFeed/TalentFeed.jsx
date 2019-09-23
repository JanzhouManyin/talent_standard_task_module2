import React from 'react';
import ReactDOM from 'react-dom';
import Cookies from 'js-cookie'
import { AuthenticatingBanner } from '../Layout/Banner/AuthenticatingBanner.jsx';
import { LoggedInNavigation } from '../Layout/LoggedInNavigation.jsx';
import LoggedInBanner from '../Layout/Banner/LoggedInBanner.jsx';
import TalentCard from '../TalentFeed/TalentCard.jsx';
import TalentProfile from '../TalentFeed/CompanyProfile.jsx';
import FollowingSuggestion from '../TalentFeed/FollowingSuggestion.jsx';
import Opportunity from './Opportunity.jsx';
import CompanyProfile from '../TalentFeed/CompanyProfile.jsx';

import { BodyWrapper, loaderData } from '../Layout/BodyWrapper.jsx';

export default class TalentFeed extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            loadNumber: 5,
            loaderData: loaderData,
            feedData: [],
            watchlist: [],
            
            loadingFeedData: false,
            companyDetails: []
        }
        this.init = this.init.bind(this);
    };

    init() {
        let loaderData = this.state.loaderData;
        //loaderData.allowedUsers.push("Talent");
        loaderData.isLoading = false;
        this.setState({ loaderData, })
    }

    componentDidMount() {
        this.loadData();
        this.init();
        window.addEventListener('scroll', this.handleScroll);
    };

    handleScroll() {
        const win = $(window);
        if ((($(document).height() - win.height()) == Math.round(win.scrollTop())) || ($(document).height() - win.height()) - Math.round(win.scrollTop()) == 1) {
            $("#load-more-loading").show();
            //load ajax and update states
            //call state and update state;
        }
    };
    loadData() {
        var cookies = Cookies.get('talentAuthToken');
        $.ajax({
            url: 'http://localhost:60290/profile/profile/getEmployerProfile',
            headers: {
                'Authorization': 'Bearer ' + cookies,
                'Content-Type': 'application/json'
            },
            type: "GET",
            contentType: "application/json",
            dataType: "json",
            success: function (res) {
                let employerData = null;
                if (res.employer) {
                    employerData = res.employer
                    //console.log("employerData", employerData)
                }
                console.log("success");
                console.log(res);
                this.updateWithoutSave(employerData)
            }.bind(this),
            error: function (res) {
                console.log(res.status)
            }
        })
        this.init()
    }
    updateWithoutSave(newData) {

        let newSD = Object.assign({}, this.state.companyDetails, newData)

        this.setState({
            companyDetails: newSD
        })
    }
    render() {
        return (
            <BodyWrapper reload={this.init} loaderData={this.state.loaderData}>
                <div className="ui grid talent-feed container">
                    <div className="four wide column">
                        <CompanyProfile
                            controlFunc={this.updateForComponentId}
                            companyDetails={this.state.companyDetails}
                            componentId='companyDetails'
                        />
                    </div>
                    <div className="eight wide column">
                        <Opportunity />
                     
                        
                        <p id="load-more-loading">
                            <img src="/images/rolling.gif" alt="Loading…" />
                        </p>
                    </div>
                    <div className="four wide column">
                        <div className="ui card">
                            <FollowingSuggestion />
                        </div>
                    </div>
                </div>
            </BodyWrapper>
        )
    }
}