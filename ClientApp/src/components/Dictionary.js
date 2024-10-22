import React, {Component} from 'react';
import Plus from '@mui/icons-material/PlusOne';
import Button from '@mui/material/Button';
import { RowDictionary } from './RowDictionary';
import { ModalMessage } from './ModalMessage';
import '../css/ModalInputKey.css';
import { ModalInput } from './ModalInput';
import { REQUEST_URLS } from '../Constants'

export class Dictionary extends Component {

    constructor(props) {
        super(props);
        this.state = {
            dict: [], 
            loading: true,
            showModalMessage: false,
            modalMessage: "",
            modalInputOpen: false,
            modalInputValue: ""
        }; 
    }

    async populateDetails() {
        const response = 
            await fetch(`${REQUEST_URLS.Details}=${this.props.language}`);
            const data = await response.json();
        if  (response.status!=200){
            console.log(`got error ${response.status} with msg ${data}`);
            this.setState({
                showModalMessage: true, 
                modalMessage: `${data} `,
                loading: false
            });
        } else {
            this.setState({ dict: data})
            this.setState({loading: false });
        }
      }

    componentDidMount() {
        this.populateDetails();
    }

    onClickPlus() {
        this.setState({modalInputOpen: true, modalInputValue: ""});
    }

    async addRequest(arg) {
        var response = await fetch(REQUEST_URLS.AddKey, {
            
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({newKey: arg})    
            
        });
        const data = await response.json();
        if (response.status==200) {
            this.populateDetails();
        } else {
            console.log(`got error ${response.status}`);
            this.setState({
                showModalMessage: true, 
                modalMessage: `${data} `
            });
        }
    }
    
    closeModalMessage() {
        this.setState({showModalMessage: false});
    }

    closeInput(newValue) {
        console.log("Dictionary closeInput");
        this.setState({modalInputOpen: false, modalInputValue: ""});
        this.addRequest(newValue);
    }

    cancelInput() {
        console.log("Dictionary cancelInput");
        this.setState({modalInputOpen: false, modalInputValue: ""});
    }

    renderDetails() {
        return (
            <div> 
                <Button variant='contained' 
                    onClick={this.onClickPlus.bind(this)} >
                    <Plus/>
                </Button>
                
                    <ModalInput 
                        modalInputOpen={this.state.modalInputOpen}
                        modalValue={this.state.modalInputValue} 
                        closeInput={this.closeInput.bind(this)}
                        cancelInput={this.cancelInput.bind(this)}
                        caption="Enter key name"
                    >
                    </ModalInput>

                <table className="table table-striped" aria-labelledby="tableLabel">
                    <thead>
                        <tr>
                            <th style={{width:'50%'}}>Key</th>
                            <th style={{width:'50%'}}>Value</th>
                        </tr>
                    </thead>
                    <tbody>
                    {this.state.dict.map(row=>
                        <RowDictionary 
                            language={this.props.language}
                            key = {row.key.keyValue}
                            keyName = {row.key.keyValue}
                            value = {row.value} 
                        />
                    )}
                    </tbody>
            </table>
            <ModalMessage 
                modalOpen={this.state.showModalMessage}
                message = {this.state.modalMessage}
                closeMessage = {this.closeModalMessage.bind(this)}>
            </ModalMessage>
        </div>
        );
    }

    render() {

        let contents = this.state.loading
            ? <p><em>Loading...</em></p>
            : this.renderDetails();

        return (
            <div>
                {contents}
            </div>
        );
       
    }
}