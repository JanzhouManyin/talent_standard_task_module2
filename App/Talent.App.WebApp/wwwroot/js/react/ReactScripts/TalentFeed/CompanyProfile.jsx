import React from 'react';
import { Loader } from 'semantic-ui-react';

export default class CompanyProfile extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            companyContact:[],
            skills: [],
            location: [],
            default_Content:"We currently do not have specific skills that we desire",
            show_file: false,
        }
    }

    componentWillReceiveProps(nextProps) {
        var companyDetails = nextProps.companyDetails;
        var location = companyDetails.companyContact.location;
        var skills = companyDetails.skills;
        var show_file = false;
        if (skills == null) {
            show_file = false;
        } else {
            show_file = true;
        }
 
        this.setState({
            companyContact: companyDetails.companyContact,
            skills: skills,
            location: location,
            show_file: show_file
        })
    }

    render() {
        const CompanyName = this.state.companyContact.name;
        const Phone = this.state.companyContact.phone;
        const Email = this.state.companyContact.email;
        const Skills = this.state.companyContact.skills;
        const Country = this.state.location.country;
        const City = this.state.location.city;
        const show_file = this.state.show_file;
        const default_Content = this.state.default_Content;
        return (
            <div className="ui card">
                <div className="extra content">
                    <div className="center aligned author">
                        <img className="ui avatar image" src="http://semantic-ui.com/images/avatar/small/jenny.jpg" />
                    </div>
                </div>
                <div className="content">
                    <div className="center aligned header">{CompanyName}</div>
                    <div className="center aligned description">
                        <div><i className="map marker alternate icon"></i>{City}&nbsp;{Country}</div>        
                    </div>
                    <div className="center aligned description">
                        {show_file ? Skills : default_Content}
                        <p>We currently do not have specific skills that we desire</p>
                    </div>
                    <div className="center aligned description">
                        <div><i className="phone volume icon"></i>:{Phone} </div>
                    </div>
                    <div className="center aligned description">
                        <div><i className="envelope outline icon"></i>:{Email} </div>
                    </div>
                </div>
                
            </div>
        )
        
    }
}