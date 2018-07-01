 import { Injectable } from '@angular/core';
import Linq = require("./linq");
import Ontapmodels = require("../models/ontap.models");
import IElement = Ontapmodels.IElement;
import { Http, Response, Headers, RequestOptions, URLSearchParams } from '@angular/http';
import { Observable }     from 'rxjs/Observable';
import './rxjs.operators.ts';
import {IStronglyTypedEvents, EventDispatcher, IEvent} from './StronglyTypedEvents';
import { FormsModule }   from '@angular/forms';
import { LoginService } from "../components/login/login.service";
import { Locale, LocaleService, LocalizationService } from 'angular2localization';
import { CloudinaryOptions, CloudinaryUploader } from 'ng2-cloudinary';

export enum ProcessingStatus {
    None,
    Loading,
    Saving,
    Adding,
    Deleting,
    Importing
}

export class Options {
    value: any;
    label: string;

    constructor(value: any, label: string) {
        this.value = value;
        this.label = label;
    }
}

export class LabeledCloudinaryUploader extends CloudinaryUploader {
    constructor(options: CloudinaryOptions, public label: string = 'image') {
        super(options);
    }
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

    constructor(protected elmService: TService, public locale: LocaleService, public localization: LocalizationService, public uploaders: LabeledCloudinaryUploader[] = []) {
        super(locale, localization);
        this.isBrowser = typeof (document) != "undefined";
        this.get();

        for (let uploader of uploaders) {
            //Override onSuccessItem function to record cloudinary response data
            uploader.onSuccessItem = (item: any, response: string, status: number, headers: any) => {
                //response is the cloudinary response
                //see http://cloudinary.com/documentation/upload_images#upload_response
                const cloudinaryImage = JSON.parse(response);
                if (this.editing) {
                    var e: any = this.editing;
                    e[uploader.label] = cloudinaryImage.public_id;
                }
                if (this.adding) {
                    var a: any = this.adding;
                    e[uploader.label] = cloudinaryImage.public_id;
                }

                return { item, response, status, headers };
            };

            //Override onSuccessItem function to record cloudinary response data
            uploader.onErrorItem = (item: any, response: string, status: number, headers: any) => {
                //response is the cloudinary response
                //see http://cloudinary.com/documentation/upload_images#upload_response
                console.error(response);
                return { item, response, status, headers };
            };

            uploader.onCompleteAll = () => {
                if (!this.hasSomethingToUpload()) {
                    this.editing ? this.save() : this.add();
                }
            }
        }
    }

    get() {
        this.status = ProcessingStatus.Loading;
        this.elmService.get()
            .subscribe(
            elements => {
                this.elements = elements;
                this.onLoadSignal(elements);
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
        if (this.hasSomethingToUpload()) {
            this.uploadAll();
        } else {
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
    }

    edit(id) {
        const element = new Linq.List(this.elements).Where(e => e.id === id).First();
        this.editing = this.elmService.new(element);
    }

    cancelEdit() {
        this.editing = null;
        this.adding = null;
    }

    delete(replacement = false) {
        if (!this.editing) { return; }
        this.status = ProcessingStatus.Deleting;
        this.processingId = this.editing.id;
        this.elmService.delete(this.editing, replacement)
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
                this.onDeleteSignal(element, replacement);
                this.status = ProcessingStatus.None;
                this.processingId = null;
            },
            error => {
                this.errorMessage = error;
                this.status = ProcessingStatus.None;
                this.processingId = null;
            });
        this.editing = null;
    }

    save() {
        if (!this.editing) { return; }
        this.status = ProcessingStatus.Saving;
        this.processingId = this.editing.id;
        if (this.hasSomethingToUpload()) {
            this.uploadAll();
        } else {
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
    }

    startAdd() {
        this.adding = this.elmService.default();
    }

    hasSomethingToUpload() {
        return !this.uploaders.every((u) => { return u && u.getNotUploadedItems().length < 1 });
    }

    uploadAll() {
        this.uploaders.forEach((u) => { u.uploadAll() });
    }

    private _load = new EventDispatcher<AppComponent<TInterface, TService>, TInterface[]>();

    public get onLoad(): IEvent<AppComponent<TInterface, TService>, TInterface[]> {
        return this._load.asEvent();
    }

    public onLoadSignal(elements: TInterface[]) {
        if (elements) {
            this._load.dispatch(this, elements);
        }
    }

    private _delete = new EventDispatcher<AppComponent<TInterface, TService>, [TInterface, any]>();

    public get onDelete(): IEvent<AppComponent<TInterface, TService>, [TInterface, any]> {
        return this._delete.asEvent();
    }

    public onDeleteSignal(element: TInterface, replacement: any) {
        if (element) {
            this._delete.dispatch(this, [element, replacement]);
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
 export abstract class RoAppService<TInteface> {
     constructor(protected http: Http, protected loginService: LoginService) {
         //console.log("Login service instantiated: " + loginService);
     }

     protected serverUrl; // URL to web API

     protected  extractData(res: Response) {
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

     get(): Observable<TInteface[]> {
         return this.http.get(this.serverUrl, this.options)
             .map(this.extractData)
             .catch(this.handleError);
     }

     getOne(id: any): Observable<TInteface> {
         return this.http.get(this.serverUrl + "/" + id, this.options)
             .map(this.extractData)
             .catch(this.handleError);
     }
 }

@Injectable()
export abstract class AppService<TInterface extends IElement> extends RoAppService<TInterface> {
    constructor(protected http: Http, protected loginService: LoginService) {
        super(http, loginService);
    }

    add(pub: TInterface): Observable<TInterface> {
        let body = JSON.stringify(pub);
        return this.http.post(this.serverUrl, body, this.options)
            .map(this.extractData, this)
            .catch(this.handleError);
    }

    change(element: TInterface): Observable<TInterface> {
        let body = JSON.stringify(element);
        return this.http.put(this.serverUrl + "/" + element.id, body, this.options)
            .map(this.extractData)
            .catch(this.handleError);
    }

    delete(element: TInterface, replacement: any): Observable<TInterface> {
        var query = this.serverUrl + "/" + element.id;
        let options = this.options;
        if (replacement) {
            let params: URLSearchParams = new URLSearchParams();
            params.set('replacementId', replacement);
            options.search = params;
        }
        return this.http.delete(query, options)
            .map(this.extractData)
            .catch(this.handleError);
    }

    abstract new(element: TInterface): TInterface;
    copy(source: TInterface, destination: TInterface): void {
        for (let key in source) {
            if (source.hasOwnProperty(key)) {
                destination[key] = source[key];
            }
        }
    }
    abstract default(): TInterface;
}

