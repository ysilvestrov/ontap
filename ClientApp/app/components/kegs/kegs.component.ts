import * as ng from '@angular/core';
import { Http } from '@angular/http';
import { List } from "../../modules/linq";
import {IPub, IUser, IKeg, KegStatus} from "../../models/ontap.models";
import {EPubService} from "../epubs/epubs.service";
import {KegService} from "./kegs.service";
import {AppComponent, AppService} from "../../modules/appComponent";
import { Locale, LocaleService, LocalizationService } from "angular2localization";

@ng.Component({
    selector: 'kegs',
    providers: [KegService],
  template: require('./kegs.component.html')
})
export class KegsComponent extends  AppComponent<IKeg, KegService> {
    public pubs: IPub[];

    constructor(elmService: KegService, public locale: LocaleService, public localization: LocalizationService) {
        super(elmService, locale, localization);
    }
}