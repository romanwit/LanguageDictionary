import React, {Component} from 'react';
import Edit from '@mui/icons-material/Edit';
import Clear from '@mui/icons-material/Clear';
import Button from '@mui/material/Button';
import '../css/ModalInputKey.css';
import { ModalInput } from './ModalInput';
import { REQUEST_URLS } from '../Constants'

export class RowDictionary extends Component {

    constructor(props) {
        super(props);
        this.state = {
            edit: false, 
            value: this.props.value,
            lastUpdatedValue: this.props.value,
            language: this.props.language,
            keyName: this.props.keyName,
            okCallback:this.onClick.bind(this),
            cancelCallback:this.onCancel.bind(this),
            modalValue: this.props.value,
            caption: "Enter key value"
        };
    }

    editValue() {
        this.setState({
            edit: true,
            modalValue: this.state.value, 
            okCallback:this.onClick.bind(this),
            cancelCallback:this.onCancel.bind(this),
            caption: "Enter value"
        });
    }

    editValueRequest(args) {
        fetch(REQUEST_URLS.EditValue, {
            
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(args)    
            
        }).then(response=>response.json().
        then(data => 
            ({
                status: response.status, 
                body: data
            }))).
        then((data)=> {
            if (data.status==200) {
                this.setState({
                    lastUpdatedValue: data.body.value,
                    value: data.body.value
                });
            } else {
                console.log(`got error ${data.status} with msg ${data.body}`);
 
            }
        }).catch((error) => alert(`Response LanguagesList returned ${error}`));
    }

    onClick(newValue) {
        this.editValueRequest({
            KeyValue: this.state.keyName, 
            LanguageValue: this.state.language,
            Value: newValue
        });
        this.setState({edit: false});
    }

    onCancel() {
        this.setState({
            edit: false,
            value: this.state.lastUpdatedValue
        });
    }

    onDeleteKey() {
        this.deleteKeyRequest({Key: this.state.keyName});
    }

    onEditKey() {
        this.setState({
            edit: true,
            modalValue: this.state.keyName, 
            okCallback:this.onClickEditKey.bind(this),
            cancelCallback:this.onCancelEditKey.bind(this),
            caption: "Enter key value"
        });
    }

    onClickEditKey(newKey) {
        this.editKeyRequest({
            OldKey: this.state.keyName, 
            NewKey: newKey 
        });
        this.setState({edit: false});
    }

    deleteKeyRequest(args) {
        fetch(REQUEST_URLS.DeleteKey, {
            
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(args)    
            
        }).then(response=>response.json().
        then(data => 
            ({
                status: response.status, 
                body: data
            }))).
        then((data)=>{
            if (data.status==200) {
                this.props.updateWholeDictionary();
            } else {
                console.log(`got error ${data.status} with msg ${data.body}`);
            }
        }).catch((error) => alert(`Response DeleteKey returned ${error}`));
    }

    editKeyRequest(args) {
        fetch(REQUEST_URLS.EditKey, {
            
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(args)    
            
        }).then(response=>response.json().
        then(data => 
            ({
                status: response.status, 
                body: data
            }))).
        then((data)=>{
            if (data.status==200) {
                this.setState({
                    keyName: data.body.keyValue
                });
            } else {
                console.log(`got error ${data.status} with msg ${data.body}`);
            }
        }).catch((error) => alert(`Response EditKey returned ${error}`));
    }

    onCancelEditKey() {
        this.setState({
            edit: false
        });
    }

    render() {

        return <tr>
            <td>
                <Button variant='contained' 
                    onClick={this.onEditKey.bind(this)}>
                    <Edit/>
                </Button>
                &nbsp;
                <Button variant='contained' 
                    onClick={this.onDeleteKey.bind(this)}>
                    <Clear/>
                </Button>
                &nbsp;
                {this.state.keyName}
            </td>
            <td>
            <div>
                <Button variant='contained'
                    onClick={this.editValue.bind(this)}>
                    <Edit/>
                </Button>
                &nbsp;&nbsp;
                {this.state.value}
                <ModalInput 
                        modalInputOpen={this.state.edit}
                        modalValue={this.state.modalValue} 
                        closeInput={this.state.okCallback}
                        cancelInput={this.state.cancelCallback}
                        caption = {this.state.caption}
                    ></ModalInput>
            </div>
            </td>
        </tr>
    }
}