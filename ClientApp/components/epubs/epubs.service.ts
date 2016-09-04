import { Injectable }     from '@angular/core';
import { Http, Response, Headers, RequestOptions } from '@angular/http';
import {IPub} from "../../models/ontap.models.ts";
import { Observable }     from 'rxjs/Observable';
// Add the RxJS Observable operators we need in this app.
import '../../modules/rxjs.operators.ts';

@Injectable()
export class EPubService {
    constructor(private http: Http) {}

    private serverUrl = 'api/pubs'; // URL to web API

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


    add(pub: IPub): Observable<IPub> {
        let body = JSON.stringify(pub);
        let headers = new Headers({ 'Content-Type': 'application/json' });
        let options = new RequestOptions({ headers: headers });
        return this.http.post(this.serverUrl, body, options)
            .map(this.extractData)
            .catch(this.handleError);
        //return new Observable<IPub>(o => {
        //    o.next(pub);
        //    o.complete();
        //});
    }

    change(pub: IPub): Observable<IPub> {
        let body = JSON.stringify(pub);
        let headers = new Headers({ 'Content-Type': 'application/json' });
        let options = new RequestOptions({ headers: headers });
        return this.http.put(this.serverUrl + "/" + pub.id, body, options)
            .map(this.extractData)
            .catch(this.handleError);
        //return new Observable<IPub>(o => {
        //    o.next(pub);
        //    o.complete();
        //});
    }

    get(): Observable<IPub[]> {
        return this.http.get(this.serverUrl)
            .map(this.extractData)
            .catch(this.handleError);
        //return new Observable<IPub[]>(o => {
        //    o.complete();
        //});
    }
}