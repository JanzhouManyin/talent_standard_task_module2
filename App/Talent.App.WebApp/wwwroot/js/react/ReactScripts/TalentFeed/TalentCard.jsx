import React from 'react';
import ReactPlayer from 'react-player';
import PropTypes from 'prop-types'
import { Popup, Icon } from 'semantic-ui-react'

export default class TalentCard extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            showDetails: true
        };
        this.handleChangeView = this.handleChangeView.bind(this);
    };

    handleChangeView() {
        this.setState({
            showDetails: this.state.showDetails ? false : true
        })

    }
    
    render() {
        return (
            this.state.showDetails ? this.renderDetails() : this.renderVideo()
        )
    }

    renderVideo() {
        return (
            <div className="ui card fluid">
                <div clasName="content">
                    <video width="100%" controls>

                        Your browser does not support HTML5 video.
                    </video>
                </div>
                <div className="extra content eight wide column">

                    <div>
                        <i className="middle big user icon" onClick={this.handleChangeView}></i>
                    </div>
                    <div >
                        <i className="big file pdf outline icon"></i>
                    </div>
                    <div>
                        <i className="big linkedin alternate icon"></i>
                    </div>
                    <div>
                        <i className="big github icon"></i>
                    </div>
                </div>
            </div>
        )

    }

    renderDetails() {
        const { name,
            currentEmpolyment,
            visa,
            skills } = this.props.data;

        return (
            <div className="ui fluid card items">
                <div className="content">
                    <i className="right floated star icon"></i>
                    <div className="header">{name}</div>
                </div>
                <div className="item">
                    <div className="image">
                        <img
                            className="ui large image"
                            src="https://react.semantic-ui.com/images/avatar/large/matthew.png" />
                    </div>
                    <div className="content">
                        <div>
                            <h4>Talent Snapshort</h4>
                        </div>
                        <div className="meta">
                        </div>
                        <div ><b>CURRENT EMPLOYER</b> <br />
                            <span>{currentEmpolyment}</span>
                        </div>
                        <div><b>VISA STATUS</b> <br />
                            <span>{visa}</span>
                        </div>
                        <div><b>POSITION</b> <br />
                            <span></span>
                        </div>
                    </div>
                </div>
                <div className="extra">
                    <div >
                        <i className="middle big video icon" onClick={this.handleChangeView}></i>
                    </div>
                    <div>
                        <i className="big file pdf outline icon"></i>
                    </div>
                    <div >
                        <i className="big linkedin alternate icon"></i>
                    </div>
                    <div >
                        <i className="big github icon"></i>
                    </div>
                </div>
                <div className="extra">
                    {skills.length > 0 && skills.map(s =>
                        <button className="ui blue basic button">{s.name}</button>
                    )}
                </div>
            </div>
        )
    }
}

