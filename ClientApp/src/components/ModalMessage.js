import React, { Component } from "react";
import Modal from '@mui/material/Modal';
import Button from '@mui/material/Button';
import Check from '@mui/icons-material/Check';
import Box from '@mui/material/Box';
import Typography from '@mui/material/Typography';
import '../css/ModalMessage.css';

export class ModalMessage extends Component {

    constructor(props) {
        console.log("ModalMessage constructor called");
        super(props);
        this.state = {
            modalOpen: this.props.modalOpen
        };
    }

    close() {
        this.setState({modalOpen: false})
        this.props.closeMessage();
    }

    handleKeyDown(evt) {
        console.log("ModalMessage handleKeyDown");
        if ((evt.key === 'Enter')||(evt.key === 'Escape')) {
            setTimeout(()=>{
                this.close();
            },0);
        }
    }

    componentDidUpdate(prevProps) {
        if (this.props.modalOpen && ! prevProps.modalOpen) {
            this.setState({modalOpen: true});
        } else if (!this.props.modalOpen && prevProps.modalOpen) {
            this.setState({modalOpen: false});
        } 

   }

    render() {
        return (<Modal open={this.state.modalOpen} 
        
            onKeyDown={evt=>this.handleKeyDown(evt)}>
                    <Box 
                        textAlign={"center"} 
                        className = "modal-message-box"
                    >
                       
                    <Typography id="modal-modal-description" sx={{ mt: 2 }}>
                        &nbsp;{this.props.message}&nbsp;
                    </Typography>
                    
                    <Button onClick={this.close.bind(this)}
                        variant="contained">
                        <Check/>
                    </Button>
                    
                    
                    </Box>
                </Modal>
            );
    }
}