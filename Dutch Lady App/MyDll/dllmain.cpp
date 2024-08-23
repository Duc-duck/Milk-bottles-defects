// dllmain.cpp : Defines the entry point for the DLL application.
#include "pch.h"
#include "Source_OpenCV.h"


EXPORTED_METHOD void cvt2BGR(unsigned char* buffer_data, ImageInfo& image_BGR, int Bitmap_width, int Bitmap_height)
{
    cv::Mat image = cv::Mat(Bitmap_height, Bitmap_width, CV_8UC1, buffer_data, cv::Mat::AUTO_STEP);
    cv::Mat img;
    cv::cvtColor(image, img, cv::COLOR_GRAY2BGR);

    image_BGR.size = img.total() * img.elemSize();
    image_BGR.elementSize = img.elemSize();
    image_BGR.data = (unsigned char*)calloc(image_BGR.size, sizeof(unsigned char));
    std::copy(img.datastart, img.dataend, image_BGR.data);
}

EXPORTED_METHOD double Rectangularity(unsigned char* imageBuffer, ImageInfo& out_img, double threshold, double threshold2,
                                    int Bitmap_width, int Bitmap_height, double x, double x1, double y, double y1, bool draw)
{
    cv::Mat image = cv::Mat(Bitmap_height, Bitmap_width, CV_8UC1);
    image.data = imageBuffer;
    cv::Mat ROI(image, cv::Rect(x * Bitmap_width, y * Bitmap_height,
        (int)((x1 - x) * Bitmap_width), (int)((y1 - y) * Bitmap_height)));
    cv::Mat crop, image_bgr;
    ROI.copyTo(crop);
    cvtColor(image, image_bgr, cv::COLOR_GRAY2BGR);
    cv::Mat thresh;
    cv::inRange(crop, 1, threshold, thresh);
    Mat Points, thresh1, crop_thresh, rotate_image;
    Rect Min_Rect1;
    double result = 0;
    if (countNonZero(thresh) > 200)
    {
        cv::dilate(crop, crop, getStructuringElement(cv::MORPH_RECT, cv::Size(5, 1)));
        cv::erode(crop, crop, getStructuringElement(cv::MORPH_RECT, cv::Size(5, 1)));
        cv::bitwise_and(crop, crop, crop_thresh, thresh);
        cv::inRange(crop, 1, threshold2, thresh1);
        cv::erode(thresh1, thresh1, getStructuringElement(cv::MORPH_RECT, cv::Size(5, 5)));
        cv::dilate(thresh1, thresh1, getStructuringElement(cv::MORPH_RECT, cv::Size(5, 5)));     
        if (cv::countNonZero(thresh1) > 200)
        {
            std::vector<cv::Point> points;
            cv::findNonZero(thresh1, points);
            cv::RotatedRect rotateRect = cv::minAreaRect(points);
            cv::Point2f rect_points[4];
            std::vector<cv::Point2f> points_select;
            rotateRect.points(rect_points);
            float angle = getAngle(rect_points, points_select);
            cv::Point2f center((crop_thresh.cols - 1) / 2.0, (crop_thresh.rows - 1) / 2.0);
            cv::Mat rotation_matrix = cv::getRotationMatrix2D(center, -angle, 1.0);
            cv::warpAffine(crop_thresh, rotate_image, rotation_matrix, crop_thresh.size());
            std::vector<cv::Point> rotate_points;
            cv::findNonZero(rotate_image, rotate_points);
            cv::Rect rect = cv::boundingRect(rotate_points);
            cv::Point p1, p2;
            p1.x = rect.tl().x;
            p1.y = rect.tl().y;
            p2.x = rect.br().x;
            p2.y = rect.tl().y + (rect.height / 6);
            if (p1.y < 0) p1.y = 1;
            cv::Mat ROI_1(rotate_image, cv::Rect(p1.x, p1.y, p2.x - p1.x, p2.y - p1.y));
            cv::Mat roi1, thresh2;
            ROI_1.copyTo(roi1);
            cv::inRange(roi1, 1, threshold, thresh2);
            cv::Mat points_final;
            cv::findNonZero(thresh2, points_final);
            //std::vector<cv::Point> hull;
            //cv::convexHull(points_final, hull);
            //result = (double)countNonZero(thresh2) / cv::contourArea(hull);

            cv::Rect Min_Rect1 = cv::boundingRect(points_final);
            result = (double)countNonZero(thresh2) / (double)Min_Rect1.area();
            if (draw)
            {
                cv::line(image_bgr, points_select[0] + cv::Point2f(x * Bitmap_width, y * Bitmap_height),
                                    points_select[1] + cv::Point2f(x * Bitmap_width, y * Bitmap_height),
                                    cv::Scalar(0, 255, 0), 1);
                std::vector <std::vector<cv::Point >> contours;
                std::vector<cv::Vec4i> hierarchy;
                cv::findContours(thresh2, contours, hierarchy, cv::RETR_TREE, cv::CHAIN_APPROX_SIMPLE);
                ContoursInfo RetContour = GetMaxContours(contours);
                for (size_t p = 0; p < contours[RetContour.ContourId].size(); ++p)
                {
                    contours[RetContour.ContourId][p] = contours[RetContour.ContourId][p] + cv::Point(p1.x, p1.y);
                }
                cv::Mat zero_mat = cv::Mat::zeros(crop_thresh.size(), CV_8UC1), rotate_contour;
                cv::drawContours(zero_mat, contours, RetContour.ContourId, 255, 1);
                cv::Mat rotate_back_matrix = cv::getRotationMatrix2D(center, angle, 1.0);
                cv::warpAffine(zero_mat, rotate_contour, rotate_back_matrix, zero_mat.size());
                cv::findContours(rotate_contour, contours, hierarchy, cv::RETR_TREE, cv::CHAIN_APPROX_SIMPLE);
                for (size_t p = 0; p < contours[0].size(); ++p)
                {
                    contours[0][p] = contours[0][p] + cv::Point(x * Bitmap_width, y * Bitmap_height);
                }
                cv::drawContours(image_bgr, contours, 0, cv::Scalar(0, 0, 255), 2);
            }
        }
        else
        {
            findNonZero(thresh, Points);
            Rect Min_Rect = boundingRect(Points);
            cv::Point p1, p2; // p1 is top-left, p2 is bottom-right
            p1.x = Min_Rect.tl().x;
            p1.y = Min_Rect.tl().y;
            p2.x = Min_Rect.br().x;
            p2.y = Min_Rect.tl().y + (Min_Rect.height / 6);
            if (p1.y < 0) p1.y = 1; // p1.y can be negative if "Min_Rect.tl().y" equals 0
            cv::Mat ROI_1(crop, cv::Rect(p1.x, p1.y, p2.x - p1.x, p2.y - p1.y));
            cv::Mat roi1;
            ROI_1.copyTo(roi1);
            cv::dilate(roi1, roi1, getStructuringElement(cv::MORPH_RECT, cv::Size(5, 1)));
            cv::erode(roi1, roi1, getStructuringElement(cv::MORPH_RECT, cv::Size(5, 1)));
            cv::threshold(roi1, thresh1, threshold, 255, THRESH_BINARY_INV);
            Mat Points1;
            findNonZero(thresh1, Points1);
            Min_Rect1 = boundingRect(Points1);
            if (draw)
            {
                vector<vector<Point>> contours;
                vector<Vec4i> hierarchy;
                findContours(thresh1, contours, hierarchy, RETR_TREE, CHAIN_APPROX_SIMPLE);
                ContoursInfo RetContour = GetMaxContours(contours);
                Point PointOffset;
                PointOffset.x = x * Bitmap_width + p1.x;
                PointOffset.y = y * Bitmap_height + p1.y;
                for (size_t p = 0; p < contours[RetContour.ContourId].size(); ++p)
                {
                    contours[RetContour.ContourId][p] = contours[RetContour.ContourId][p] + PointOffset;
                }
                drawContours(image_bgr, contours, RetContour.ContourId, Scalar(0, 0, 255), 3);
            }
            result = (double)countNonZero(thresh1) / (double)Min_Rect1.area();
        }
    }

    out_img.size = image_bgr.total() * image_bgr.elemSize();
    out_img.elementSize = image_bgr.elemSize();
    out_img.data = (unsigned char*)calloc(out_img.size, sizeof(unsigned char));
    std::copy(image_bgr.datastart, image_bgr.dataend, out_img.data);

    return result;
}

EXPORTED_METHOD double Water_Level(unsigned char* imageBuffer, ImageInfo& out_img, double threshold, double ratio_otsu_water,
                                   int BitmapWidth, int BitmapHeight, double x_lid, double x1_lid, double y_lid, double y1_lid,
                                   double x, double x1, double y, double y1, bool draw)
{
    cv::Mat image = cv::Mat(BitmapHeight, BitmapWidth, CV_8UC1);
    image.data = imageBuffer;
    cv::Mat crop, crop_lid, image_bgr;
    cvtColor(image, image_bgr, cv::COLOR_GRAY2BGR);

    cv::Mat ROI_lid(image, cv::Rect(x_lid * BitmapWidth, y_lid * BitmapHeight,
        (int)((x1_lid - x_lid) * BitmapWidth), (int)((y1_lid - y_lid) * BitmapHeight)));
    ROI_lid.copyTo(crop_lid);
    cv::Mat thresh_lid;
    cv::threshold(crop_lid, thresh_lid, threshold, 255, THRESH_BINARY_INV);
    if (countNonZero(thresh_lid) > 300)
    {
        cv::Mat ROI(image, cv::Rect(x * BitmapWidth, y * BitmapHeight,
            (int)((x1 - x) * BitmapWidth), (int)((y1 - y) * BitmapHeight)));
        ROI.copyTo(crop);
        cv::Mat thresh, reduced;
        cv::threshold(crop, thresh, threshold, 255, THRESH_BINARY_INV);
        if (cv::countNonZero(thresh) < 100)
        {
            out_img.size = image_bgr.total() * image_bgr.elemSize();
            out_img.elementSize = image_bgr.elemSize();
            out_img.data = (unsigned char*)calloc(out_img.size, sizeof(unsigned char));
            std::copy(image_bgr.datastart, image_bgr.dataend, out_img.data);
            return 0.0;
        }
        bitwise_and(crop, thresh, reduced);
        Mat Blur_img, thresh1;
        GaussianBlur(reduced, Blur_img, Size(5, 5), 3, 0);
        thresh1 = Mat::zeros(Blur_img.size(), Blur_img.type());
        int threshValue = Find_ThreshValue(Blur_img, countNonZero(Blur_img), 2, ratio_otsu_water);
        cv::parallel_for_(cv::Range(0, 3), Custom_Threshold(Blur_img, thresh1, 1, threshValue));
        vector<vector<Point>> contours, align_vector;
        vector<Vec4i> hierarchy;
        findContours(thresh1, contours, hierarchy, RETR_TREE, CHAIN_APPROX_SIMPLE);
        if (!contours.empty())
        {
            ContoursInfo RetContour = GetMaxContours(contours);
            if (draw)
            {
                Point Point99;
                Point99.x = x * BitmapWidth;
                Point99.y = y * BitmapHeight;
                for (size_t p = 0; p < contours[RetContour.ContourId].size(); ++p)
                {
                    contours[RetContour.ContourId][p] = contours[RetContour.ContourId][p] + Point99;
                }
                drawContours(image_bgr, vector<vector<Point>>(1, contours[RetContour.ContourId]), -1, Scalar(0, 0, 255), -1);
            }

            out_img.size = image_bgr.total() * image_bgr.elemSize();
            out_img.elementSize = image_bgr.elemSize();
            out_img.data = (unsigned char*)calloc(out_img.size, sizeof(unsigned char));
            std::copy(image_bgr.datastart, image_bgr.dataend, out_img.data);

            return RetContour.Area / (double)((x1 - x) * BitmapWidth * (y1 - y) * BitmapHeight);
        }
        else
        {
            out_img.size = image_bgr.total() * image_bgr.elemSize();
            out_img.elementSize = image_bgr.elemSize();
            out_img.data = (unsigned char*)calloc(out_img.size, sizeof(unsigned char));
            std::copy(image_bgr.datastart, image_bgr.dataend, out_img.data);
            return 0.0;
        }
    }
    else
    {
        out_img.size = image_bgr.total() * image_bgr.elemSize();
        out_img.elementSize = image_bgr.elemSize();
        out_img.data = (unsigned char*)calloc(out_img.size, sizeof(unsigned char));
        std::copy(image_bgr.datastart, image_bgr.dataend, out_img.data);
        return 2.0;
    }
}

EXPORTED_METHOD Deform Wrinkles (unsigned char* imageBuffer, ImageInfo& out_img, ushort min_ed, ushort max_ed, ushort min_ra,
                                ushort max_ra, double min_cir_ratio, double shrink_seal, int BitmapWidth, int BitmapHeight, 
                                double x, double y, double x1, double y1, bool draw)
{
    cv::Mat image = cv::Mat(BitmapHeight, BitmapWidth, CV_8UC1);
    image.data = imageBuffer;

    cv::Mat crop, image_bgr, edge_image, edge_ellipse, edge_image_strict_ori, crop_edge, mask_outer_cir, crop_circle2;
    std::vector<std::vector<cv::Point>> contours, contours_big_circle_ellipse, contours1, contour_ellipse,
                                        contours_big_circle, contour_cir_shrink(1);
    std::vector<cv::Vec4i> hierarchy, hierarchy_ellipse;
    cv::RotatedRect rect, rect_1;
    float radius = 0, radius1 = 0;
    cv::Point2f center, center1;
    std::vector<cv::Vec3f> circle;
    Deform output{};
    output.circularity = 1.0;
    output.wrinkle_area = 0;
    cvtColor(image, image_bgr, COLOR_GRAY2BGR);
    cv::Mat ROI(image, cv::Rect(x * BitmapWidth, y * BitmapHeight,
        (int)((x1 - x) * BitmapWidth), (int)((y1 - y) * BitmapHeight)));    
    ROI.copyTo(crop);

    cv::Mat zero_mat = cv::Mat::zeros(crop.size(), CV_8UC1);
    mask_outer_cir = zero_mat.clone();
    edge_ellipse = zero_mat.clone();

    cv::HoughCircles(crop, circle, cv::HOUGH_GRADIENT, 1,
        crop.rows + 100, 60, 30, min_ra * shrink_seal, max_ra * shrink_seal);
    if (circle.size() > 0)
    {
        size_t max{};
        for (size_t i = 0; i < circle.size(); i++)
        {
            if (i > 0)
            {
                if (circle[i][2] > circle[i - 1][2])
                    max = i;
            }
            else max = 0;
        }
        cv::Vec3i c = circle[max];
        center = cv::Point(c[0], c[1]);
        radius = c[2];
        cv::Canny(crop, edge_image_strict_ori, min_ed * 2, max_ed * 2, 3, true);
        cv::Mat mask1 = zero_mat.clone();
        cv::circle(mask1, center, radius * min_cir_ratio, 255, -1, cv::LINE_AA);
        cv::bitwise_and(edge_image_strict_ori, edge_image_strict_ori, crop_circle2, mask1);
        output.wrinkle_area = cv::countNonZero(crop_circle2);
        if (draw)
        {
            cv::Point2f center_final = center + cv::Point2f(x * BitmapWidth, y * BitmapHeight);
            cv::circle(image_bgr, center_final, radius, cv::Scalar(0, 255, 255), 1);
        }
    }

    cv::Canny(crop, edge_image, min_ed, max_ed, 3, true);
    cv::findContours(edge_image, contours, hierarchy, cv::RETR_TREE, cv::CHAIN_APPROX_NONE);
    for (std::vector<cv::Point> contour : contours)
    {
        if (outer_radius(contour) > min_ra && outer_radius(contour) < max_ra && roundness(contour) > 0.85)
        {
            contours_big_circle.push_back(contour); //////// OK if there is no contour found
        }
    }
    if (contours_big_circle.size() > 0)
    {
        ContoursInfo RetContour = GetMaxContours(contours_big_circle);
        rect = cv::fitEllipse(contours_big_circle[RetContour.ContourId]);
        cv::ellipse(edge_ellipse, rect, 255, 1);
        cv::findContours(edge_ellipse, contours_big_circle_ellipse, hierarchy, cv::RETR_TREE, cv::CHAIN_APPROX_NONE);
        if (cv::arcLength(contours_big_circle[RetContour.ContourId], false) > cv::arcLength(contours_big_circle_ellipse[RetContour.ContourId], false) * 0.85)
        {            
            cv::findContours(edge_image_strict_ori, contours, hierarchy, cv::RETR_TREE, cv::CHAIN_APPROX_SIMPLE);
            for (std::vector<cv::Point> contour : contours)
            {
                if (cv::arcLength(contour, false) > 100)
                {
                    contours1.push_back(contour);
                }
            }
            cv::Mat edge_image_strict = zero_mat.clone();
            cv::drawContours(edge_image_strict, contours1, -1, 255, 1);
            contour_cir_shrink[0] = Deform_Contour(contours_big_circle_ellipse[0], 0.89);
            cv::drawContours(mask_outer_cir, contour_cir_shrink, 0, 255, -1);
            cv::bitwise_and(edge_image_strict, edge_image_strict, crop_edge, mask_outer_cir);
            cv::Mat crop_points;
            cv::findNonZero(crop_edge, crop_points);
            std::vector<std::vector<cv::Point>> hull(1);
            cv::convexHull(crop_points, hull[0]);

            rect_1 = cv::fitEllipse(hull[0]);           
            cv::Mat min_hull_ellipse = zero_mat.clone();
            cv::ellipse(min_hull_ellipse, rect_1, 255, -1);
            cv::findContours(min_hull_ellipse, contour_ellipse, hierarchy_ellipse, cv::RETR_TREE, cv::CHAIN_APPROX_SIMPLE);
            cv::minEnclosingCircle(contour_ellipse[0], center1, radius1);
            cv::Mat temp1 = zero_mat.clone();
            cv::circle(temp1, center1, radius1, 255, -1);
            output.circularity = (float)cv::countNonZero(min_hull_ellipse) / cv::countNonZero(temp1);

            if (draw)
            {
                cv::findContours(crop_circle2, contours, hierarchy, cv::RETR_TREE, cv::CHAIN_APPROX_SIMPLE);
                for (int i = 0; i < contours.size(); i++)
                {
                    for (int p = 0; p < contours[i].size(); ++p)
                    {
                        contours[i][p] = contours[i][p] + cv::Point(x * BitmapWidth, y * BitmapHeight);
                    }
                }
                for (int p = 0; p < hull[0].size(); ++p)
                {
                    hull[0][p] = hull[0][p] + cv::Point(x * BitmapWidth, y * BitmapHeight);
                }
                cv::RotatedRect rect_2 = cv::fitEllipse(hull[0]);
                cv::Point2f center1_ori = center1 + cv::Point2f(x * BitmapWidth, y * BitmapHeight);
                for (int i = 0; i < contours_big_circle.size(); i++)
                {
                    for (int p = 0; p < contours_big_circle[i].size(); ++p)
                    {
                        contours_big_circle[i][p] = contours_big_circle[i][p] + cv::Point(x * BitmapWidth, y * BitmapHeight);
                    }
                }                

                cv::drawContours(image_bgr, contours, -1, cv::Scalar(0, 0, 255), 1);
                cv::ellipse(image_bgr, rect_2, cv::Scalar(255,0,255), 2);
                cv::drawContours(image_bgr, contours_big_circle, -1, cv::Scalar(255, 0, 0), 2);
                cv::circle(image_bgr, center1_ori, radius1, cv::Scalar(0,255,0), 1);
            }           
        }
    }
    out_img.size = image_bgr.total() * image_bgr.elemSize();
    out_img.elementSize = image_bgr.elemSize();
    out_img.data = (unsigned char*)calloc(out_img.size, sizeof(unsigned char));
    std::copy(image_bgr.datastart, image_bgr.dataend, out_img.data);

    return output;
}

EXPORTED_METHOD void ReleaseMemoryFromC(unsigned char* buf)
{
    if (buf == NULL) return;
    free(buf);
}

BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved )
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
    case DLL_PROCESS_DETACH:
        break;
    }
    return TRUE;
}

