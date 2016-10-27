import {IPub, ICity, Pub, IServe} from "../../models/ontap.models.ts";

import { Pipe, PipeTransform } from "@angular/core";

@Pipe({
    name: "sortByTap"
})
export class SortByTap implements PipeTransform {
    transform(array: Array<IServe>, args: string): Array<IServe> {
        array.sort((a: IServe, b: IServe) => {
            if (a.tap < b.tap) {
                return -1;
            } else if (a.tap > b.tap) {
                return 1;
            } else {
                return 0;
            }
        });
        return array;
    }
}