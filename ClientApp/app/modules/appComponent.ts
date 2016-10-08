import { Injectable }     from '@angular/core';
import Linq = require("./linq.ts");
import Ontapmodels = require("../models/ontap.models");
import IElement = Ontapmodels.IElement;
import { Http, Response, Headers, RequestOptions } from '@angular/http';
import { Observable }     from 'rxjs/Observable';
import './rxjs.operators.ts';
import {IStronglyTypedEvents, EventDispatcher, IEvent} from './StronglyTypedEvents.ts';
import { FormsModule }   from '@angular/forms';
import { LoginService } from "../components/login/login.service";

@Injectable()
export class AppComponent<TInterface extends IElement, TService extends AppService<TInterface>> {
    public elements: TInterface[];
    public editing: TInterface;
    public adding: TInterface;
    public errorMessage: any;

    constructor(private elmService: TService) {
        this.get();
    }

    get() {
        this.elmService.get()
            .subscribe(
            elements => {
                this.elements = elements;
                this.signal(elements);
            },
            error => this.errorMessage = <any>error);
    }

    add() {
        if (!this.adding) { return; }
        this.elmService.add(this.adding)
            .subscribe(
            element => {
                this.elements.push(element);
                this.adding = null;
            },
            error => this.errorMessage = <any>error);
    }

    edit(id) {
        const element = new Linq.List(this.elements).Where(e => e.id === id).First();
        this.editing = this.elmService.new(element);
    }

    cancelEdit() {
        this.editing = null;
        this.adding = null;
    }

    delete() {
        if (!this.editing) { return; }
        this.elmService.delete(this.editing)
            .subscribe(
            element => {
                for (let c of this.elements) {
                    if (c.id === element.id) {
                        var index = this.elements.indexOf(c, 0);
                        if (index > -1) {
                            this.elements.splice(index, 1);
                        }
                    }
                }
            },
            error => this.errorMessage = <any>error);
        this.editing = null;
    }

    save() {
        if (!this.editing) { return; }
        this.elmService.change(this.editing)
            .subscribe(
            element => {
                for (let c of this.elements) {
                    if (c.id === element.id) {
                        this.elmService.copy(element, c);
                    }
                }
            },
            error => this.errorMessage = <any>error);
        this.editing = null;
    }

    startAdd() {
        this.adding = this.elmService.default();
    }

    private _load = new EventDispatcher<AppComponent<TInterface, TService>, TInterface[]>();

    public get onLoad(): IEvent<AppComponent<TInterface, TService>, TInterface[]> {
        return this._load.asEvent();
    }

    public signal(elements: TInterface[]) {
        if (elements) {
            this._load.dispatch(this, elements);
        }
    }
}

@Injectable()
export abstract class AppService<TInteface extends IElement> {
    constructor(private http: Http, private loginService: LoginService) {
        console.log("Login service instantiated: "+loginService);
    }

    protected serverUrl; // URL to web API

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

    private get options(): RequestOptions {
        let headers = new Headers({ 'Content-Type': 'application/json' });
        if (this.loginService.accessToken) {
            headers.append('Authorization', `Bearer ${this.loginService.accessToken.accessToken}`);
        }
        return new RequestOptions({ headers: headers });
    }

    add(pub: TInteface): Observable<TInteface> {
        let body = JSON.stringify(pub);
        return this.http.post(this.serverUrl, body, this.options)
            .map(this.extractData, this)
            .catch(this.handleError);
    }

    change(element: TInteface): Observable<TInteface> {
        let body = JSON.stringify(element);
        return this.http.put(this.serverUrl + "/" + element.id, body, this.options)
            .map(this.extractData)
            .catch(this.handleError);
    }

    delete(element: TInteface): Observable<TInteface> {
        return this.http.delete(this.serverUrl + "/" + element.id, this.options)
            .map(this.extractData)
            .catch(this.handleError);
    }

    get(): Observable<TInteface[]> {
        return this.http.get(this.serverUrl, this.options)
            .map(this.extractData)
            .catch(this.handleError);
    }

    getOne(id:any): Observable<TInteface> {
        return this.http.get(this.serverUrl + "/" + id, this.options)
            .map(this.extractData)
            .catch(this.handleError);
    }



    abstract new(element: TInteface): TInteface;
    abstract copy(source: TInteface, destination: TInteface): void;
    abstract default(): TInteface;
}