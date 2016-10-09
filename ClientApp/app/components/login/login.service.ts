import { Injectable }     from '@angular/core';
import { IUser, User, AccessToken } from "../../models/ontap.models";
import { Http, Response, Headers, RequestOptions } from '@angular/http';
import { Observable }     from 'rxjs/Observable';
import '../../modules/rxjs.operators.ts';
import {IStronglyTypedEvents, EventDispatcher, IEvent} from '../../modules/StronglyTypedEvents.ts';
import { FormsModule }   from '@angular/forms';
import * as moment from 'moment';

@Injectable()
export class LoginService {
    constructor(private http: Http) {
        try {            
            if (sessionStorage) {
                var token = sessionStorage.getItem("token");
                if (token) {
                    token = JSON.parse(token);
                    if (!(token instanceof String) && this.isValid(token)) {
                        this.accessToken = token;
                    }
                }
            }
        } catch (e) {
            //do nothing
        } 
    }

    protected serverUrl = "/api/jwt"; // URL to web API

    public currentUser: IUser;
    public accessToken: AccessToken;  

    private extractData(res: Response) {
        let body = res.json();
        return body || [];
    }

    private handleError(error: any) {
        // In a real world app, we might use a remote logging infrastructure
        // We'd also dig deeper into the error to get a better message
        let errMsg = (error.message)
            ? error.message
            : error.status ? `${error.status} - ${error.statusText}` : 'Server error';
        console.error(errMsg); // log to console instead
        return Observable.throw(errMsg);
    }

    login(login:string, password:string): Observable<AccessToken> {
        let body = JSON.stringify({name:login, password:password});
        let headers = new Headers({ 'Content-Type': 'application/json' });
        let options = new RequestOptions({ headers: headers });
        return this.http.post(this.serverUrl, body, options)
            .map(this.extractData, this)
            .catch(this.handleError);
    }

    isAuthorised(): boolean {
        return this.accessToken != null && this.isValid(this.accessToken);
    }

    isValid(token: AccessToken): boolean {
        return moment(token.expiresAt).diff(moment()) > 0;
    }


}