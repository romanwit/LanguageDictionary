import React, {Component} from 'react';
import Modal from '@mui/material/Modal';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Check from '@mui/icons-material/Check';
import Close from '@mui/icons-material/Close';

export class ModalInput extends Component {

    constructor(props) {
        super(props);
        this.state = {
            modalOpen: this.props.modalInputOpen, 
            modalValue: this.props.modalValue
        };
    }

    handleChangeModalValue(evt) {
        const val = evt.target.value;
        this.setState({
            modalValue: val
        })
    }

    handleKeyDown(evt) {
        if (evt.key === 'Enter') {
            setTimeout(()=>{
                console.log('ModalInput Enter caught');
                this.setState({
                    modalOpen: false,
                    modalValue: this.props.modalValue
                });
                this.props.closeInput(this.state.modalValue);
            }, 0);

        }
        if (evt.key === 'Escape') {
            setTimeout(()=>{
                console.log("Escape caught");
                this.setState({
                    modalOpen: false,
                    modalValue: this.props.modalValue
                });
                this.props.cancelInput();
            }, 0);

        }
    }

    handleCloseModal() {
        
        setTimeout(()=>{
            this.props.closeInput(this.state.modalValue);
            this.setState({
                modalOpen: false,
                modalValue: this.props.modalValue
            });
        }, 0);

    }

    handleCancel() {
        setTimeout(()=>{
            console.log("ModalInput handleCancel");
            this.props.cancelInput();
            this.setState({
                modalOpen: false,
                modalValue: this.props.modalValue
            });
        }, 0);
    }
   
    componentDidUpdate(prevProps) {
        if (this.props.modalInputOpen && ! prevProps.modalInputOpen) {
            this.setState({modalOpen: true});
        } else if (!this.props.modalInputOpen && prevProps.modalInputOpen) {
            this.setState({modalOpen: false});
        } 
        if (this.props.modalValue != prevProps.modalValue) {
            this.setState({modalValue: this.props.modalValue});
        }

   }

    render() {
        
        return (
        <Modal open={this.state.modalOpen}>
            <Box className="modal-box">
                <b style={{width:'90%', marginLeft: '2%', textAlign:'center'}}>
                    {this.props.caption}
                </b>
                <div style={{width:'100%', textAlign: 'center'}}>
                    <input 
                        type="text" 
                        autoFocus 
                        onChange={evt=>this.handleChangeModalValue(evt)} 
                        onKeyDown={evt=>this.handleKeyDown(evt)}
                        value={this.state.modalValue}
                        style={{height:38, width:'95%'}}>
                    </input>
                    </div>
                    <div style={{display: 'flex', justifyContent: 'center'}}>
                        <div style={{
                            display: 'flex', 
                            justifyContent: 'space-between', 
                            left: '10%', 
                            textAlign: 'center', 
                            marginTop: 10,
                            width: '90%'
                            }}>
                            <Button 
                                variant='contained'
                                onClick={this.handleCloseModal.bind(this)}
                                style={{height:45}}
                            >
                                <Check/>
                            </Button>
                            
                            <Button variant='contained'
                                onClick={this.handleCancel.bind(this)}
                                style={{height:45}}
                            >
                                <Close/>
                            </Button>
                        </div>
                    </div>
            </Box>
        </Modal>);
    }
}