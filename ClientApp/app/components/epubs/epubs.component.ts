import * as ng from '@angular/core';
import { Http } from '@angular/http';
import { List } from "../../modules/linq";
import {IPub, ICity, Pub} from "../../models/ontap.models";
import {EPubService} from "./epubs.service";
import { CityService } from "../cities/cities.service";
import { LoginService } from "../login/login.service";
import {AppComponent, AppService} from "../../modules/appComponent";
import { Locale, LocaleService, LocalizationService } from "angular2localization";
import { CloudinaryOptions, CloudinaryUploader } from 'ng2-cloudinary';
import { FileSelectDirective, FileDropDirective, FileUploader}  from 'ng2-file-upload';

@ng.Component({
    selector: 'epubs',
    providers: [EPubService, CityService],
    styles: [require('./epubs.component.css')],
  template: require('./epubs.component.html')
})
export class EPubsComponent extends  AppComponent<IPub, EPubService> {
    public cities: ICity[];
    cloudinaryImage: any;

    cloudinaryOptions: CloudinaryOptions = new CloudinaryOptions({
        cloud_name: 'ontap-in-ua',
        upload_preset: 'ontapInUa_pubs',
        autoUpload: true
    });

    uploader: CloudinaryUploader = new CloudinaryUploader(this.cloudinaryOptions);
    successMessage: string;


    constructor(elmService: EPubService,
        private loginService: LoginService, 
        private cityService: CityService,
        public locale: LocaleService,
        public localization: LocalizationService) {
        super(elmService, locale, localization);
        this.getCities();

        //Override onSuccessItem function to record cloudinary response data
        this.uploader.onSuccessItem = (item: any, response: string, status: number, headers: any) => {
            //response is the cloudinary response
            //see http://cloudinary.com/documentation/upload_images#upload_response
            this.cloudinaryImage = JSON.parse(response);
            if (this.editing)
                this.editing.image = this.cloudinaryImage.public_id;
            if (this.adding)
                this.adding.image = this.cloudinaryImage.public_id;

            return { item, response, status, headers };
        };
    }

    startAdd() {
        super.startAdd();
        this.adding.city = new List(this.cities).First();
    }

    getCities() {
        this.cityService.get()
            .subscribe(
            cities => this.cities = cities,
            error => this.errorMessage = <any>error);
    }

    onEditChangeCity(id) {
        this.onChangeCity(this.editing, id);
    }

    onAddChangeCity(id) {
        this.onChangeCity(this.adding, id);
    }

    onChangeCity(obj: IPub, id:string) {
        obj.city = new List(this.cities).Where(c => c.id === id).First();
    }

    import(id: string) {
        this.elmService.import(id)
            .subscribe(
            message => {
                this.successMessage = message;
            },
            error => this.errorMessage = <any>error);
        this.editing = null;
    }

}