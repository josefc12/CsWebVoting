import React, {Component, component} from 'react';
import { NavMenu } from './NavMenu';

export class Test extends Component {
    static displayName = Test.name;

    constructor(props){
        super(props);
        this.state = { 
            paragraphs: [],
            removeIndex: 3,
        };
        this.fetchTest = this.fetchTest.bind(this);
        this.removeElement = this.removeElement.bind(this);
    }

    fetchTest (index){
        if(index === 4 || index === 7){
            this.setState(prevState => ({
                paragraphs: [...prevState.paragraphs, <img src={testImg} class='img' id='imgid' alt='My image' />]
              }));
        }else{
            this.setState(prevState => ({
                paragraphs: [...prevState.paragraphs, <p>Test paragraph {index}</p>]
            }));
        }
    };

    removeElement(index){
        this.setState(prevState => ({
            //Understand this
            paragraphs: prevState.paragraphs.filter((_, i) => i !== index)
        }));
    }
    
    render() {
        
        const { paragraphs } = this.state;
        const { removeIndex } = this.state;

        return(
        <div className="testdiv">
            <NavMenu />
            <h1>Test page</h1>
            <button type="button" class="btn btn-primary" onClick={() => this.fetchTest(this.state.paragraphs.length+1)}>Primary</button>
            <button type="button" class="btn btn-danger" onClick={() => this.removeElement(removeIndex)}>Danger</button>

            {paragraphs.map(x => (
                x
            ))}
        </div>
        )
    }

    
}