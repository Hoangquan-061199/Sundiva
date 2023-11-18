function saveTags(idList,idSave) {
    var valsSave = "";
    $('#' + idList).find("li").each(function () {
        valsSave += "," + $(this).text();
    })
    $("#" + idSave).val(valsSave);
}
function iptTagIdsChange(addTag, idIpt, idList, idSave) {
    $("#" + addTag).click(function () {
        var vals = $("#" + idIpt).val();
        var __arrT = [];
        if (vals.length > 0)
            __arrT = vals.split(',');
        var tags = "";
        for (i = 0; i < __arrT.length; i++) {
            tags += "<li class=\"badge badge-info\"><span onclick=\"removeTagItem(this,'" + idList + "','" + idSave + "')\" class=\"lnr lnr-trash\" aria-hidden=\"true\"></span>" + __arrT[i] + "</li>";
        }
        $('#' + idList).append(tags);
        $("#" + idIpt).val("");
        saveTags(idList,idSave);
    })

}
function removeTagItem(eleThis, idList, idSave) {
    $(eleThis).parent().remove();
    saveTags(idList,idSave);
}
