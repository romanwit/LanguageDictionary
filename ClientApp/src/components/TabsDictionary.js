import * as React from 'react';
import PropTypes from 'prop-types';
import Tabs from '@mui/material/Tabs';
import Tab from '@mui/material/Tab';
import Box from '@mui/material/Box';
import { Dictionary } from './Dictionary';
import { REQUEST_URLS } from '../Constants'


function TabsPanel (props) {
    const { children, value, index, ...other } = props;

  return (
        <div
            role="tabpanel"
            hidden={value !== index}
            id={`simple-tabpanel-${index}`}
            aria-labelledby={`simple-tab-${index}`}
            {...other}
        >
            {value === index && <Box sx={{ p: 3 }}>{children}</Box>}
        </div>
    );
}

TabsPanel.propTypes = {
    children: PropTypes.node,
    index: PropTypes.number.isRequired,
    value: PropTypes.number.isRequired,
  };

  function a11yProps(index) {
    return {
      id: `simple-tab-${index}`,
      'aria-controls': `simple-tabpanel-${index}`,
    };
  }

export default function TabsDictionary() {
    
    const [value, setValue] = React.useState(0);
    const [data, setData] = React.useState([]);
    const [loading, setLoading] = React.useState(true);

    const handleChange = (event, newValue) => {
      setValue(newValue);
    };

    React.useEffect(
        ()=>{
            console.log("TabsDictionary useEffect");
            fetch(REQUEST_URLS.LanguagesList).
            then((response=>response.json())).
            then((json)=>{
                setData(json);
                setLoading(false);
                console.log(`useEffect: languages ${json}, ${data}`);
            });
        }, []
    );

    if(loading)
        return (
            <p><em>Loading...</em></p>
        )
        else {
            console.log(`rendering ${data}`);
            return (
        
                <Box sx={{ width: '100%' }}>
                    <Box sx={{ borderBottom: 1, borderColor: 'divider' }}>
                    <Tabs value={value} onChange={handleChange} aria-label="basic tabs example">
                        {data.map(
                            (tab, index)=><Tab key={tab} label={tab} {...a11yProps(index)} />
                        )}
                    </Tabs>
                    </Box>
                    {data.map((language, index)=>
                        <TabsPanel key={language} value={value} index={index}>
                            <Dictionary language={language}/>
                        </TabsPanel>
                    )}
                </Box>
                
            );
        }
  
}