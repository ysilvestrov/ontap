import { Injectable }     from '@angular/core';
import Linq = require("./linq");
import Ontapmodels = require("../models/ontap.models");
import IElement = Ontapmodels.IElement;
import { Http, Response, Headers, RequestOptions } from '@angular/http';
import { Observable }     from 'rxjs/Observable';
import './rxjs.operators.ts';
import {IStronglyTypedEvents, EventDispatcher, IEvent} from './StronglyTypedEvents';
import { FormsModule }   from '@angular/forms';
import { LoginService } from "../components/login/login.service";
import { Locale, LocaleService, LocalizationService } from 'angular2localization';

export enum ProcessingStatus {
    None,
    Loading,
    Saving,
    Adding,
    Deleting,
    Importing
}

@Injectable()
export class AppComponent<TInterface extends IElement, TService extends AppService<TInterface>> extends Locale {
    public elements: TInterface[];
    public editing: TInterface;
    public adding: TInterface;
    public errorMessage: any;
    public isBrowser: boolean;
    public processingId: any;
    public status: ProcessingStatus = ProcessingStatus.None;

    constructor(protected  elmService: TService, public locale: LocaleService, public localization: LocalizationService) {
        super(locale, localization);
        this.isBrowser = typeof (document) != "undefined";
        this.get();
    }

    get() {
        this.status = ProcessingStatus.Loading;
        this.elmService.get()
            .subscribe(
            elements => {
                this.elements = elements;
                this.signal(elements);
                this.status = ProcessingStatus.None;
            },
            error => {
                this.errorMessage = <any>error;
                this.status = ProcessingStatus.None;
            });
    }

    add() {
        if (!this.adding) { return; }
        this.status = ProcessingStatus.Adding;
        this.processingId = this.adding.id;
        this.elmService.add(this.adding)
            .subscribe(
            element => {
                this.elements.push(element);
                this.adding = null;
                this.status = ProcessingStatus.None;
                this.processingId = null;
            },
            error => {
                this.errorMessage = <any>error;
                this.status = ProcessingStatus.None;
                this.processingId = null;
            });
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
        this.status = ProcessingStatus.Deleting;
        this.processingId = this.editing.id;
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
                this.status = ProcessingStatus.None;
                this.processingId = null;
            },
            error => {
                this.errorMessage = <any>error;
                this.status = ProcessingStatus.None;
                this.processingId = null;
            });
        this.editing = null;
    }

    save() {
        if (!this.editing) { return; }
        this.status = ProcessingStatus.Saving;
        this.processingId = this.editing.id;
        this.elmService.change(this.editing)
            .subscribe(
            element => {
                for (let c of this.elements) {
                    if (c.id === element.id) {
                        this.elmService.copy(element, c);
                    }
                }
                this.status = ProcessingStatus.None;
                this.processingId = null;
            },
            error => {
                this.errorMessage = <any>error;
                this.status = ProcessingStatus.None;
                this.processingId = null;
            });
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

    public isImporting(id) {
        return this.status === ProcessingStatus.Importing && this.processingId === id;
    }

    public isSaving(id) {
        return this.status === ProcessingStatus.Saving && this.processingId === id;
    }

    public isDeleting(id) {
        return this.status === ProcessingStatus.Deleting && this.processingId === id;
    }

    public isAdding(id) {
        return this.status === ProcessingStatus.Saving;
    }
}

@Injectable()
export abstract class AppService<TInteface extends IElement> {
    constructor(protected http: Http, private loginService: LoginService) {
        console.log("Login service instantiated: "+loginService);
    }

    protected serverUrl; // URL to web API

    private extractData(res: Response) {
        let body = res.json();
        return body || [];
    }

    protected handleError(error: any) {
        // In a real world app, we might use a remote logging infrastructure
        // We'd also dig deeper into the error to get a better message
        let errMsg = (error.message)
            ? error.message
            : error.status ? `${error.status} - ${error.statusText}` : 'Server error';
        console.error(errMsg); // log to console instead
        return Observable.throw(errMsg);
    }

    protected get options(): RequestOptions {
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
    copy(source: TInteface, destination: TInteface): void {
        for (let key in source) {
            if (source.hasOwnProperty(key)) {
                destination[key] = source[key];
            }
        }
    }
    abstract default(): TInteface;
}