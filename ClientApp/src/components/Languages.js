import React, { useState } from "react";
import Plus from '@mui/icons-material/PlusOne';
import Button from '@mui/material/Button';
import '../css/ModalInputKey.css';
import { ModalInput } from './ModalInput';
import { ModalMessage } from './ModalMessage';
import { REQUEST_URLS } from '../Constants'

const Languages = (newLanguage) => {

    const [dict, setDict] = useState([]);
    const [loading, setLoading] = useState(false);
    const [modalInputOpen, setModalInputOpen] = useState(false);
    const [modalInputValue, setModalInputValue] = useState("");
    const [showModalMessage, setShowModalMessage] = useState(false);
    const [modalMessage, setModalMessage] = useState("");

    const populateLanguages = async () => {
       
        fetch(REQUEST_URLS.LanguagesList).
        then((response=>response.json())).
        then((json)=>{
            setDict(json);
            setLoading(false);
        });
    }

    const addLanguageRequest = async (newLanguage) => {
        var response = await fetch(REQUEST_URLS.AddLanguage, {
            
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({Language: newLanguage})    
            
        });
        const data = await response.json();
        if (response.status==200) {
            populateLanguages();
        } else {
            console.log(`got error ${response.status}`);
            setShowModalMessage(true);
            setModalMessage(data);
        }
    }

    const onClickPlus = () => {
        setModalInputOpen(true);
        setModalInputValue("");
    }

    const closeInput = (newLanguage) => {
        setModalInputOpen(false);
        addLanguageRequest(newLanguage);
    }

    const cancelInput = () => {
        setModalInputOpen(false);
    }

    const closeModalMessage = () => {
        setShowModalMessage(false);
    }

    React.useEffect(
        ()=>{
            populateLanguages();
        }, []
    );
    
    if(loading) {
            return (
                <p><em>Loading...</em></p>
            )
        }
        else {
            console.log(`rendering ${dict}`);
            return (
                <div>
                    <Button variant='contained' 
                        onClick={onClickPlus}>
                        <Plus/>
                    </Button>
                    <table className="table table-striped">
                        <thead>
                            <tr>
                                <th>
                                    Language
                                </th>
                            </tr>
                        </thead>
                        <tbody>
                            {
                                dict.map(l=>(
                                    <tr key={l}>
                                        <td>
                                            {l}
                                        </td>
                                    </tr> 
                                ))
                            }
                        </tbody>
                    </table>
                    <ModalInput 
                        modalInputOpen={modalInputOpen}
                        modalValue={modalInputValue} 
                        closeInput={closeInput}
                        cancelInput={cancelInput}
                        caption="Enter language name"
                    >
                    </ModalInput>
                    <ModalMessage 
                        modalOpen={showModalMessage}
                        message = {modalMessage}
                        closeMessage = {closeModalMessage}>
                    </ModalMessage>
                </div>
            );
        }
}

export default Languages;