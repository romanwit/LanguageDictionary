import React, {Component} from 'react';
import Plus from '@mui/icons-material/PlusOne';
import Folder from '@mui/icons-material/Folder';
import Button from '@mui/material/Button';
import { RowDictionary } from './RowDictionary';
import { ModalMessage } from './ModalMessage';
import '../css/ModalInputKey.css';
import { ModalInput } from './ModalInput';
import { REQUEST_URLS } from '../Constants';
import * as XLSX from 'xlsx';

export class Dictionary extends Component {

    constructor(props) {
        super(props);
        this.state = {
            dict: [], 
            loading: true,
            showModalMessage: false,
            modalMessage: "",
            modalInputOpen: false,
            modalInputValue: "",
            folder: ""
        }; 
        this.fileInputRef = React.createRef();
    }

    populateDetails() {
        fetch(`${REQUEST_URLS.Details}=${this.props.language}`).
        then(response=>response.json().
                then(data => 
                    ({
                        status: response.status, 
                        body: data
                    }))).
        then((data)=>{
            if  (data.status!=200){
                console.log(`got error ${data.status} with msg ${data.body}`);
                this.setState({
                    showModalMessage: true, 
                    modalMessage: `${data.body} `,
                    loading: false
                });
            } else {
                this.setState({ dict: data.body})
                this.setState({loading: false });
            }
        }).
        catch((error) => alert(`Response LanguagesList returned ${error}`));
        
       
      }

    componentDidMount() {
        this.populateDetails();
    }

    onClickPlus() {
        this.setState({modalInputOpen: true, modalInputValue: ""});
    }

    addRequest(arg) {
        fetch(REQUEST_URLS.AddKey, {
            
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({newKey: arg})    
            
        }).then(response=>response.json().then(
            data=>({
                status: response.status,
                body:data
            })
        )).then(data=>{
            if (data.status==200) {
                this.populateDetails();
            } else {
                console.log(`got error ${data.status}`);
                this.setState({
                    showModalMessage: true, 
                    modalMessage: `${data.body}`
                });
            }
        }).catch((error) => alert(`Response addKey returned ${error}`));
        
        
    }
    
    closeModalMessage() {
        this.setState({showModalMessage: false});
    }

    closeInput(newValue) {
        this.setState({modalInputOpen: false, modalInputValue: ""});
        this.addRequest(newValue);
    }

    cancelInput() {
        this.setState({modalInputOpen: false, modalInputValue: ""});
    }

    handleFileChange(event) {
        console.log(event.target.files[0].name);
        this.setState({folder: event.target.files[0].name});
    }

    handleClick() {
        this.fileInputRef.current.click();
    }



    renderDetails() {
        return (
            <div> 
                <Button variant='contained' 
                    onClick={this.onClickPlus.bind(this)} >
                    <Plus/>
                </Button>
                <input type="file" 
                    webkitdirectory="true" 
                    ref={this.fileInputRef}
                    style={{ display: 'none' }}
                    onChange={this.handleFileChange.bind(this)} 
                    onInput={this.handleFileChange.bind(this)} />
                
                {this.state.Folder}&nbsp;
                <Button variant='contained' 
                    style={{display:'none'}}
                    onClick={this.handleClick.bind(this)}>
                    <Folder/>
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
                            updateWholeDictionary = {this.populateDetails.bind(this)}
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