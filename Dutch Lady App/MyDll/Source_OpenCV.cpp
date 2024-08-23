#include "pch.h"
#include "Source_OpenCV.h"


int Find_ThreshValue(Mat inputImage, int totalPixels, int thresh_default, double ratio_otsu_water)
{
    // Compute histogram
    long double hist[256];

    // initialize all intensity values to 0
    for (int i = 0; i < 256; i++)
        hist[i] = 0;

    // calculate the no of pixels for each intensity values
    for (int y = 0; y < inputImage.rows; y++)
        for (int x = 0; x < inputImage.cols; x++)
            hist[(int)inputImage.at<uchar>(y, x)]++;
    hist[0] = 0;

    // Initialize variables for optimal threshold
    double maxVariance = 0, mu0 = 0, mu1 = 0, variance = 0;
    double optimalThreshold = 0;
    double sum = 0, sum1 = 0, w0 = 0, w1 = 0;
    for (int i = 0; i <= 255; i++)
    {
        sum = sum + hist[i] * i;
    }
    // Iterate through all possible threshold values
    for (int th = 0; th < 255; th++)
    {
        w0 += hist[th];
        if (w0 == 0) continue;
        w1 = totalPixels - w0;

        sum1 += th * hist[th];
        mu0 = sum1 / w0;
        mu1 = (sum - sum1) / w1;
        // Compute between-class variance
        variance = w0 * w1 * (mu0 - mu1) * (mu0 - mu1) / (sum);
        if (variance > maxVariance)
        {
            maxVariance = variance;
            optimalThreshold = th;
        }
    }
    if (maxVariance < (totalPixels * ratio_otsu_water)) return thresh_default;
    return optimalThreshold;
}

void Custom_Threshold::operator()(const cv::Range& range) const
{
    for (int i = range.start; i < range.end; i++)
    {
        cv::Mat in(img, cv::Rect(0, (img.rows / range.end) * i,
            img.cols, i == (range.end - 1) ? ((img.rows / range.end) + (img.rows % range.end)) : (img.rows / range.end)));
        cv::Mat out(retVal, cv::Rect(0, (retVal.rows / range.end) * i,
            retVal.cols, i == (range.end - 1) ? ((retVal.rows / range.end) + (retVal.rows % range.end)) : (retVal.rows / range.end)));

        for (int r = 0; r < in.rows; r++)
        {
            // We obtain a pointer to the beginning of row r
            unsigned char* ptr_in = in.ptr<unsigned char>(r);
            unsigned char* ptr_out = out.ptr<unsigned char>(r);
            for (int c = 0; c < in.cols; c++)
            {
                if (ptr_in[c] >= min && ptr_in[c] <= max)
                    ptr_out[c] = 255;
                else
                    ptr_out[c] = 0;
            }
        }
    }
}

ContoursInfo GetMaxContours(std::vector<std::vector<cv::Point>> contours)
{
    ContoursInfo RetContour;
    RetContour.Area = 0;
    RetContour.ContourId = -1;
    for (int i = 0; i < contours.size(); i++)
    {
        if (cv::contourArea(contours[i]) > RetContour.Area)
        {
            RetContour.Area = cv::contourArea(contours[i]);
            RetContour.ContourId = i;
        }
    }
    return RetContour;
}

void Auto_Canny(cv::Mat Input, cv::Mat& Output, float sigma_min, float sigma_max)
{
    float median = get_median(Input);
    int a = 5, b = (1.0 - sigma_min) * median, c = (1.0 + sigma_max) * median;
    int lower = std::max(0, b);
    int upper = std::min(255, c);
    cv::Canny(Input, Output, lower, upper, 3, true);
}

cv::Point getContour_Center(std::vector<cv::Point> contour)
{
    cv::Point center{};
    if (contour.size() > 0)
    {
        cv::RotatedRect rect = cv::minAreaRect(contour);
        center = rect.center;
    }
    return center;
}

void moveContours(std::vector<std::vector<cv::Point>>& contours, cv::Point move)
{
    if (contours.size() > 0)
    {
        for (size_t i = 0; i < contours.size(); ++i)
        {
            for (size_t p = 0; p < contours[i].size(); ++p)
            {
                contours[i][p] = contours[i][p] + move;
            }
        }
    }
}

void moveContours(std::vector<std::vector<cv::Point>> contours, std::vector<std::vector<cv::Point>>& output, cv::Point move)
{
    if (contours.size() > 0)
    {
        for (std::vector<cv::Point> contour : contours) // contour is a deep copy of contours's elements
        {
            std::vector<cv::Point> output_contour;
            for (cv::Point point : contour)
            {
                point += move;
                output_contour.push_back(point);
            }
            output.push_back(output_contour);
        }
    }
}

float outer_radius(std::vector<cv::Point> contour) // contour was found with flag "CHAIN_APPROX_SIMPLE" causing some points reduced
{
    if (contour.size() > 0)
    {
        std::vector<cv::Point> contours_poly;
        cv::Point2f center;
        float radius;
        cv::approxPolyDP(contour, contours_poly, 3, false);
        cv::minEnclosingCircle(contours_poly, center, radius);
        return radius;
    }
    else return 0;
}

float roundness(std::vector<cv::Point> contour) // in this function we input contour, in Halcon we input object (region)
{
    if (contour.size() > 0)
    {
        double Distance_mean = 0, distance_abs = 0, Sigma_sqrt = 0, sigma_abs = 0, area_contour = 0;
        area_contour = contour.size();
        cv::Point center;
        //std::vector<cv::Moments> moments(1);
        //moments[0] = cv::moments(contour);
        //center.x = (int)(moments[0].m10 / moments[0].m00 + 1e-5);
        //center.y = (int)(moments[0].m01 / moments[0].m00 + 1e-5);
        cv::RotatedRect rect = cv::minAreaRect(contour);
        center = rect.center;

        for (cv::Point point : contour)
        {
            distance_abs += sqrt(pow(center.x - point.x, 2) + pow(center.y - point.y, 2));
        }
        Distance_mean = distance_abs / area_contour;

        for (cv::Point point : contour)
        {
            sigma_abs += pow(sqrt(pow(center.x - point.x, 2) + pow(center.y - point.y, 2)) - Distance_mean, 2);
        }
        Sigma_sqrt = sqrt(sigma_abs / area_contour);

        return (1 - (Sigma_sqrt / Distance_mean));
    }
    else return 0;
}

bool IsClosedContour(std::vector<cv::Point> contour)
{
    if (cv::contourArea(contour) > cv::arcLength(contour, true))
        return true;
    else
        return false;
}

std::vector<cv::Point> Deform_Contour(std::vector<cv::Point> contour, float shrink_or_expand)
{
    /////////////// 0 < shrink_ratio < 1.0, expand_ratio > 1.0 /////////////
    if (IsClosedContour(contour))
    {
        //std::vector<cv::Moments> moments(1);
        //moments[0] = cv::moments(contour);
        //float cx = moments[0].m10 / moments[0].m00 + 1e-5;
        //float center.y = moments[0].m01 / moments[0].m00 + 1e-5;

        cv::RotatedRect rect = cv::minAreaRect(contour);
        cv::Point center = rect.center;

        std::vector<cv::Point> shrinked_contour;
        cv::Point center_geometry;
        for (cv::Point point : contour)
        {
            float distanceX_of_shrinked = (center.x - point.x) * shrink_or_expand;
            float distanceY_of_shrinked = (center.y - point.y) * shrink_or_expand;
            cv::Point new_point;
            new_point.x = center.x - distanceX_of_shrinked;
            new_point.y = center.y - distanceY_of_shrinked;
            shrinked_contour.push_back(new_point);
        }

        return shrinked_contour;
    }
    else
    {
        std::vector<cv::Point> empty{};
        return empty;
    }
}

float getAngle(cv::Point2f* rect_points, std::vector<cv::Point2f>& points_select)
{
    std::vector<cv::Point2f> points;
    points_select.clear();
    cv::Point2f point0, point1, point3;
    float ymin = 0, ymax = 0, xmin = 0, xmax = 0, xmax_of_ymax = 0, ymax_of_xmin = 0, ymin_of_xmax = 0;
    int ymax_index = 0, xmax_of_ymax_index = 0, xmin_index, ymax_of_xmin_index = 0, xmax_index = 0, ymin_of_xmax_index = 0;
    for (int i = 0; i < 4; i++)
    {
        points.push_back(*(rect_points + i));
    }
    for (int i = 0; i < points.size(); i++)
    {
        if (points[i].x < 0.0) points[i].x = 0;
        if (points[i].y < 0.0) points[i].y = 0;
        if (points[i].x > xmax) //find the xmax
        {
            xmax = points[i].x;
            xmax_index = i;
        }
        if (points[i].y > ymax) //find the ymax
        {
            ymax = points[i].y;
            ymax_index = i;
        }
    }
    xmin = xmax;
    ymin = ymax;
    ymin_of_xmax = ymax;
    for (int i = 0; i < points.size(); i++)
    {
        if (points[i].y == ymax && points[i].x > xmax_of_ymax)
        {
            xmax_of_ymax = points[i].x;
            xmax_of_ymax_index = i;
        }

        if (points[i].x < xmin) // Find points 1
        {
            xmin = points[i].x;
            xmin_index = i;
            ymax_of_xmin = points[i].y;
            ymax_of_xmin_index = i;
        }
        if (points[i].x == xmin && points[i].y > ymax_of_xmin)
        {
            ymax_of_xmin = points[i].y;
            ymax_of_xmin_index = i;
        }

        if (points[i].x == xmax && points[i].y < ymin_of_xmax)
        {
            ymin_of_xmax = points[i].y;
            ymin_of_xmax_index = i;
        }
    }

    point0 = points[xmax_of_ymax_index];
    point1 = points[ymax_of_xmin_index];
    point3 = points[ymin_of_xmax_index];
    points_select.push_back(point0);
    //std::cout << "----- POINTS ------:\n" << point0 << "\n" << point1 << "\n" << point3 << "\n";

    if (euclideanDist(point0, point1) > euclideanDist(point0, point3))
    {
        points_select.push_back(point1);
    }
    else
    {
        points_select.push_back(point3);
    }
    if (points_select[1].x - points_select[0].x == 0)
        return 90;
    else
        return (180.0f / CV_PI) * atan((points_select[1].y - points_select[0].y) / (points_select[0].x - points_select[1].x));
    //The denominator determines whether the return value negative or positive
}

float euclideanDist(cv::Point2f& a, cv::Point2f& b)
{
    cv::Point2f diff = a - b;
    return cv::sqrt(diff.x * diff.x + diff.y * diff.y);
}
