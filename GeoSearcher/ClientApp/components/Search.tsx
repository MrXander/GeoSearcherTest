import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import 'isomorphic-fetch';

export interface SearchProps extends React.Props<any> {
    searchUrl: string;
    searchType: SearchType;
}

export enum SearchType {
    IP,
    City
}

interface SearchState {
    locations: Location[];
    searchString: string;
    searchUrl: string;
    searchType: SearchType;
    searchStringValidation: string;
    loading: boolean;
}

export class Search extends React.Component<SearchProps, SearchState> {
    constructor(props: any) {
        super(props);
        this.handleInput = this.handleInput.bind(this);
        this.state = { locations: [], loading: false, searchString: '', searchUrl: props.searchUrl, searchType: props.searchType, searchStringValidation: '' };
    }

    public render() {
        let contents = this.state.loading
            ? <p><em>Loading...</em></p>
            : this.renderLocation(this.state.locations, this.state.searchString);

        return <div>
            <p>Enter valid ip address</p>
            {contents}
        </div>;
    }

    private fetchData(str: string) {
        fetch(this.state.searchUrl + str)
            .then(response => response.json() as Promise<Location[]>)
            .then(data => {
                this.setState({ locations: data, loading: false });
            });
    }

    private handleInput(event: React.FormEvent<HTMLInputElement>) {
        var inputStr = event.currentTarget.value;
        this.setState({ searchString: inputStr });
        if (this.validateInput(inputStr))
            this.fetchData(inputStr);
        else
            this.setState({ searchStringValidation: 'Invalid input string' });
    }

    private validateInput(str: string) {
        switch (+this.state.searchType) {
            case SearchType.IP:
                console.log('Validating string ' + str + ' with regex ' + '\\b(?:\d{1,3}\.){3}\d{1,3}\\b');
                console.log(str.search(/\b(?:\d{1,3}\.){3}\d{1,3}\b/));
                return str.search(/\b(?:\d{1,3}\.){3}\d{1,3}\b/) !== -1;
            case SearchType.City:
                if (str.indexOf(" ") === -1)
                    return true;
                return false;
            default:
                return false;
        }
    }

    protected renderLocation(locations: Location[], str: string) {
        return <div>
            <div>
                <label>
                    IP: 
                    <input type="text" value={str} onChange={this.handleInput} />
                </label>
            </div>
            <table className='table'>
                <thead>
                    <tr>
                        <th>Country</th>
                        <th>Region</th>
                        <th>City</th>
                        <th>Postal</th>
                        <th>Organization</th>
                        <th>Latitude</th>
                        <th>Longitude</th>
                    </tr>
                </thead>
                <tbody>
                    {locations.map(location =>
                        <tr key={location.latitude + location.longitude}>
                            <td>{location.country}</td>
                            <td>{location.region}</td>
                            <td>{location.city}</td>
                            <td>{location.postal}</td>
                            <td>{location.organization}</td>
                            <td>{location.latitude}</td>
                            <td>{location.longitude}</td>
                        </tr>
                    )}
                </tbody>
            </table>
        </div>;
    }
}

interface Location {
    country: string;
    region: string;
    postal: string;
    city: string;
    organization: string;
    latitude: string;
    longitude: string;
}
