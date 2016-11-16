import * as ng from '@angular/core';
import { Http } from '@angular/http';
import { List } from "../../modules/linq";
import {IBrewery, Brewery , ICountry} from "../../models/ontap.models";
import {BreweryService} from "./breweries.service";
import {CountryService} from "../countries/countries.service";
import {AppComponent, AppService} from "../../modules/appComponent";
import { Locale, LocaleService, LocalizationService } from "angular2localization";
import { CloudinaryOptions, CloudinaryUploader } from 'ng2-cloudinary';
import { FileSelectDirective, FileDropDirective, FileUploader } from 'ng2-file-upload';

@ng.Component({
    selector: 'breweries',
    providers: [BreweryService, CountryService],
    styles: [require('./breweries.component.css')],
    template: require('./breweries.component.html')
})
export class BreweriesComponent extends AppComponent<IBrewery, BreweryService> {

    public countries: ICountry[];

    cloudinaryImage: any;

    cloudinaryOptions: CloudinaryOptions = new CloudinaryOptions({
        cloud_name: 'ontap-in-ua',
        upload_preset: 'ontapInUa_breweries',
        autoUpload: true
    });

    uploader: CloudinaryUploader = new CloudinaryUploader(this.cloudinaryOptions);

    constructor(elmService: BreweryService, private countryService: CountryService, public locale: LocaleService, public localization: LocalizationService) {
        super(elmService, locale, localization);
        this.getCountries();

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
        this.adding.country = new List(this.countries).First();
    }

    getCountries() {
        this.countryService.get()
            .subscribe(
            countries => this.countries = countries,
            error => this.errorMessage = <any>error);
    }

    onEditChangeCountry(id) {
        this.onChangeCountry(this.editing, id);
    }

    onAddChangeCountry(id) {
        this.onChangeCountry(this.adding, id);
    }

    onChangeCountry(obj: IBrewery, id: string) {
        obj.country = new List(this.countries).Where(c => c.id === id).First();
    }

}